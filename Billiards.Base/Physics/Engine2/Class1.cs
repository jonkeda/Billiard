using System.Collections.Generic;
using System.Linq;
using Billiards.Base.Extensions;
using com.walter.eightball.objects;
using OpenCvSharp;

namespace com.walter.eightball;

/*import com.walter.eightball.math.Vector3D
import com.walter.eightball.objects.{ Ball, Board}
import com.walter.eightball.state.PhysicsHandler.CollisionType.CollisionType

import scala.collection.mutable.{ Buffer, Map}
import scala.math._
*/
using System.Numerics;

public static class Vector3Ex
{
	public static Vector3 Create(float length, float angle)
	{
		return new Vector3(length * MathF.Cos(angle.ToRadians()), length * MathF.Sin(angle.ToRadians()), 0f);

	}

	public static Vector3 normalized(this Vector3 vector3)
	{
		return Vector3.Normalize(vector3);
	}

	public static float Angle2d(this Vector3 vector3)
	{
		return ((MathF.Atan2(vector3.Y, vector3.X) + 360f) % 360f).ToDegrees();
	}
}

public class PhysicsHandler
{
	private float g = 9.8f; //Gravitational constant
	private float cfc = 0.01f; //Coefficient of friction between two colliding balls
	private float cfs = 0.2f; //Coefficient of friction while sliding
	private float cfr = 0.01f; //Coefficient of friction while rolling
	private float cfw = 0.2f; //Coefficient of friction between the ball and the wall during a collision
	private float td = 0.0002f; //Duration of a collision between two balls
	private float separationOffset = 0.001f; //Minimum separation between objects when calling method separate

	/** Class to represent a pocket */
	private class Pocket : Ball
	{
		public Pocket(float X, float Y, float z) : base(X, Y, z, 0)
		{
			this.mass = 1f;
			this.radius = Board.PocketRadius;
		}
	}

	private Pocket[] Pockets = new Pocket[]
	{
		new Pocket(0f, 0f, 0f),
		new Pocket(Board.Width / 2f, 0f, 0f),
		new Pocket(Board.Width / 2f, Board.Height, 0f),
		new Pocket(Board.Width, 0f, 0f),
		new Pocket(0f, Board.Height, 0f),
		new Pocket(Board.Width, Board.Height, 0f)
	};

	/** Case class to represent the velocity and angular velocity of a ball */
	public class VelocityState
	{
        public Vector3 Velocity { get; }
        public Vector3 AngularVelocity { get; }

        public VelocityState(Vector3 velocity, Vector3 angularVelocity)
        {
            Velocity = velocity;
            AngularVelocity = angularVelocity;
        }
	}

	/** Enumeration to represent the type of collision */
	public enum CollisionType
	{
		BallBall,
		HorizontalWall,
		VerticalBall,
		Pocketed
	}

	/** Returns whether the sequence of balls are still or not */
	public bool areStill(IEnumerable<Ball> balls)
	{
		return balls.All(b => b.velocity.Length() < 0.0001f);
	}



	/**  Returns the resulting delta velocities of a collision
      *
      *  Both the velocity and the angular velocity of the balls
      *  are updated. The collision is assumed to be perfectly
      *  elastic, conserving both the total kinetic energy,
      *  momentum, and angular momentum. */
	public (VelocityState, VelocityState) collide(Ball ball1, Ball ball2)
	{
		var n = 1f / (ball2.location - ball1.location).Length() * (ball2.location - ball1.location);
		var nNorm = n.normalized();

		//Calculate the normal components of the velocity vectors
		var vn1 = (Vector3.Dot(ball1.velocity, (-1f * n))) * (-1f * n);
		var vn2 = Vector3.Dot(ball2.velocity, n) * n;

		//Calculate the tangential components of the velocity vectors
		var vt1 = ball1.velocity - vn1;
		var vt2 = ball2.velocity - vn2;

		/* Add the tangential component of the velocity vector with the
         * normal component of the other vector to get the resulting velocity */
		var newDVelocity1 = vn2 - vn1;
		var newDVelocity2 = vn1 - vn2;

		//Vectors to the touching points between the balls
		var r1 = ball1.radius * nNorm;
		var r2 = -1f * ball2.radius * nNorm;

		//Relative speed at the point of contact
		var vpr = Vector3.Cross(r2, ball2.angularVelocity) - Vector3.Cross(r1, ball1.angularVelocity);


		//∆v of the normal velocities before and after the collisions
		var dvn1 = signum(Vector3.Dot((vn2 - vn1), (nNorm))) * (vn2 - vn1).Length();
		var dvn2 = signum(Vector3.Dot((vn1 - vn2), (nNorm))) * (vn1 - vn2).Length();

		//Calculate the new angular speeds
		var newDAngularVelocity1 = (5f / 2f) * Vector3.Cross(r1, (1 / td * -cfc * (ball1.mass * dvn2) *
																  (vpr + vt1).normalized())) *
								   (td / (ball1.mass * MathF.Pow(ball1.radius, 2)));
		var newDAngularVelocity2 = (5f / 2f) * Vector3.Cross(r2, (1 / td * -cfc * (ball2.mass * dvn1) *
																  (vpr + vt2).normalized())) * (td /
			(ball2.mass * MathF.Pow(ball2.radius, 2)));

		return (new VelocityState(newDVelocity1, newDAngularVelocity1),
			new VelocityState(newDVelocity2, newDAngularVelocity2));
	}

	private float signum(float dot)
	{
		if (dot < 0)
		{
			return -1;
		}
		if (dot > 0)
		{
			return 1;
		}
		return 0;
	}

	/** Returns the resulting delta velocities of a wall collision
      *
      * @param ball The ball that is colliding with the wall
      * @param horizontal Whether the wall is horziontal or not (false => vertical)
      * @return A tuple containing the balls delta velocities
      */
	private VelocityState collideWall(Ball ball, bool horizontal)
	{

		//The velocity tangential to the collision plane
		var vt = (horizontal) ? ball.velocity.Y : ball.velocity.X;

		//The change in the length of the angular velocity
		var dAngularVelocityLen = cfw * MathF.Pow(vt, 2) / 2f;

		//The current length of the angular velocity
		var AVVelocityLen = ball.angularVelocity.Length();

		//The factor to dampen the angular velocity with (if the delta velocity is bigger than the velocity, stop the spin entirely)
		var c = (dAngularVelocityLen > AVVelocityLen) ? 0f : dAngularVelocityLen / AVVelocityLen;

		//The delta angular velocity (invert the rotational speed of
		var dAngularVelocity = (horizontal)
			? -c * new Vector3(-2f * ball.angularVelocity.X, ball.angularVelocity.Y, ball.angularVelocity.Z)

			: -c * new Vector3(ball.angularVelocity.X, -2f * ball.angularVelocity.Y, ball.angularVelocity.Z);


		//Deivation caused by the dampining of the spin around the z-axis during the spin
		var d = MathF.Sqrt(2.0f / 5.0f) * ball.radius * dAngularVelocity.Z;

		//Invert the velocity tangential to the collision plane and add the deviation
		Vector3 dVelocity;
		if (horizontal)
		{
			//Velocity required to stop the ball
			var stop = new Vector3(0f, -vt, 0f);

			//The velocity afterwards
			var deviation = Vector3Ex.Create(MathF.Abs(vt), new Vector3(d, -ball.velocity.Y, 0f).Angle2d());

			//Sum the two to get the delta velocity
			dVelocity = stop + deviation;
		}
		else
		{
			var stop = new Vector3(-vt, 0f, 0f);
			var deviation = Vector3Ex.Create(MathF.Abs(vt), new Vector3(-ball.velocity.X, d, 0f).Angle2d());
			dVelocity = stop + deviation;
		}

		return new VelocityState(dVelocity, dAngularVelocity);
	}

	public class Collision
	{
		public Collision(CollisionType type, Ball ball1, Ball? ball2)
		{
			Type = type;
			Ball1 = ball1;
			Ball2 = ball2;
		}

		public CollisionType Type { get; set; }
		public Ball Ball1 { get; set; }
		public Ball? Ball2 { get; set; }

	}

	public class CollisionTime
	{
		public CollisionTime(CollisionType type, float? time)
		{
			Type = type;
			Time = time;
		}

		public CollisionType Type { get; set; }
		public float? Time { get; set; }
	}

	/** Returns the time when the next collisions will occur
      * and what types of collisions that will occur
      *
      * The first element in the returned tuple specifies when
      * it's going to occur (if it will occur), the second a
      * vector containing collision type, the affected ball
      * as well as a second ball, if it's a collision with
      * another ball
      *
      * @param balls
      * @return
      */
	public (float?, List<Collision>) getNextCollisions(List<Ball> balls)
	{

		float? foundTime = null;
		var collisions = new List<Collision>();


		//Find all BallBall collisions
		for (int i = 0; i < balls.Count; i++)
		{
			for (int n = i + 1; n < balls.Count; n++)
			{
				var curTime = timeUntilCollision(balls[i], balls[n]);
				if (curTime.HasValue)
				{
					if (foundTime.HasValue)
					{
						//Find the collisions that will occur the soonest
						if (curTime < foundTime)
						// if (curTime.exists(ct => foundTime.forall(ct < _)))
						{
							foundTime = curTime;
							collisions.Clear();
							collisions.Add(new Collision(CollisionType.BallBall, balls[i], balls[n]));
						}
					}
					else if (curTime == foundTime)
					//else if (curTime.exists(ct => foundTime.forall(ct == _)))
					{
						collisions.Add(new Collision(CollisionType.BallBall, balls[i], balls[n]));
					}
				}
			}
		}

		//Find wall collisions
		foreach (Ball ball in balls)
		{
			var curCollisions = new List<CollisionTime>()
			{
				new CollisionTime(CollisionType.HorizontalWall, timeUntilHorizontalWallCollision(ball, 0f)),
				new CollisionTime(CollisionType.HorizontalWall, timeUntilHorizontalWallCollision(ball, Board.Height)),
				new CollisionTime(CollisionType.VerticalBall, timeUntilVerticalWallCollision(ball, 0f)),
				new CollisionTime(CollisionType.VerticalBall, timeUntilVerticalWallCollision(ball, Board.Width))
			};

			//Find the collisions that will occur the soonest
			if (curCollisions.Count > 0)
			{
				if (foundTime.HasValue)
				{
					foreach (CollisionTime ct in curCollisions)
					{
						if (ct.Time.HasValue)
						{
							if (ct.Time < foundTime)
							//if (curTime.exists(ct => foundTime.forall(ct < _)))
							{
								foundTime = ct.Time;
								collisions.Clear();
								collisions.Add(new Collision(ct.Type, ball, null));
							}
							else if (ct.Time == foundTime)
							{
								collisions.Add(new Collision(ct.Type, ball, null));
							}
						}
					}
				}

				/*
				 for ((coT, curTime) < -curCollisions)
				{
					if (curTime.exists(ct => foundTime.forall(ct < _)))
					{
						foundTime = curTime;
						collisions.clear();
						collisions += ((coT, ball, null));
					}
					else if (curTime.exists(ct => foundTime.forall(ct == _)))
					{
						collisions += ((coT, ball, null));
					}
				}
				*/
			}
		}

		//Find pocket collisions
		foreach (Ball ball in balls)
		{
			var curTime = timeUntilPocketed(ball);
			if (curTime.HasValue)
			{
				if (foundTime.HasValue)
				{
					//Find the collisions that will occur the soonest
					if (curTime < foundTime)
					{
						foundTime = curTime;
						collisions.Clear();
						collisions.Add(new Collision(CollisionType.Pocketed, ball, null));
					}
				}
				else if (curTime == foundTime)
				{
					collisions.Add(new Collision(CollisionType.Pocketed, ball, null));
				}
			}
		}

		return (foundTime, collisions);
	}

	/** Returns the relative velocity between the table and the touching point of the ball
 *
 *  This velocity is determined by: (ω X R) + v
 *  where: ω is the angular velocity
 *         R is a vector from the center of the ball to the touching point with the board (0, 0, -r) */
	public Vector3 getRelativeVelocity(Ball ball)
	{
		return Vector3.Cross(ball.angularVelocity, new Vector3(0f, 0f, -ball.radius)) + ball.velocity;
	}

	/** Moves the given balls according to their velocities
         *
         *  @param balls the balls to move
         *  @param t time since last execution (in seconds) */
	void moveBalls(IEnumerable<Ball> balls, float t)
	{

		foreach (Ball ball in balls)
		{
			ball.location += t * ball.velocity;
		}
	}

	/** Shoots the cue ball in the given direction according to the cue velocity and where on the ball it was hit
    *
    * (0,0) in ball position parameter designates hitting straightly in the middle of the ball. A ball position
    * outside of the ball will result in a miss (no velocity will be applied)
    *
    * Note that this part of the physics implementation is not realistic. Instead, the ball is assumed to be set
    * in motion with the same velocity as the cue stick was moving in. The resulting angular velocity
    * of the ball is determined by where the ball was hit:
    *
    * ωX = k||v||∆Y sin(Θ)
    * ωY = k||v||∆Y cos(Θ)
    * ωz = k||v||∆X */
	public void shoot(Ball cueBall, Vector3 cueVelocity, Vector3 ballPosition)
	{
		if (ballPosition.Length() < cueBall.radius)
		{
			var angle = cueVelocity.Angle2d().ToRadians();
			cueBall.velocity = cueVelocity;
			var len = cueBall.velocity.Length();
			cueBall.angularVelocity = 100f * new Vector3(len * ballPosition.Y * MathF.Sin(angle),
				len * ballPosition.Y * MathF.Cos(angle),
				3f * len * ballPosition.X);
		}
	}

	/** Returns the time until the next collision between two balls
     *
     *  The collision time is determined by treating the balls as if they were having a linear
     *  trajectory. This allows us to use a quadratic equation to determine the distance d
     *  between the balls for a certain time t. The potential collision occurs when the
     *  distance between the balls is the sum of their radii.
     *
     *  The equation is as following:
     *
     *  d(t) = t^2 (∆v.∆v) + 2t (∆r.∆v) + (∆r.∆r) - (R1+R1)^2
     *  where: ∆r is the difference in position between the balls
     *  			 R  are the radii of the balls
     *
     *  This method is based on the following source:
     *  http://twobitcoder.blogspot.fi/2010/04/circle-collision-detection.html */
	public float? timeUntilCollision(Ball ball1, Ball ball2)
	{

		var v12 = ball1.velocity - ball2.velocity;
		var r12 = ball1.location - ball2.location;

		var a = Vector3.Dot(v12, v12);
		var b = 2 * Vector3.Dot(r12, v12);
		var c = Vector3.Dot(r12, r12) - MathF.Pow(ball1.radius + ball2.radius, 2);

		//Calculate the discriminant of the equation
		var disc = MathF.Pow(b, 2) - 4 * a * c;

		if ((ball2.location - ball1.location).Length() < ball1.radius + ball2.radius)
		{
			return null;
		}
		else if (disc < 0f)
		{
			//No real solutions => no upcoming collisions
			return null;
		}
		else if (disc == 0f)
		{
			//Both velocities are zero => they won't collide
			if (2 * a == 0f || -b / (2 * a) < 0f)
			{
				return null;
			}
			else
			{
				//Otherwise calculate the collision time
				return (-b / (2 * a));
			}
		}
		else
		{
			//Two upcoming collisions, choose the next one (but not any past solutions)
			var t1 = ((-b - MathF.Sqrt(disc)) / (2 * a));
			var t2 = ((-b + MathF.Sqrt(disc)) / (2 * a));
			var t = MathF.Min((t1 < 0f) ? float.MaxValue : t1,
					(t2 < 0f) ? float.MaxValue : t2)
				;
			if (t == float.MaxValue)
				return null;
			else
				return t;
		}
	}

	/** Returns the time when the ball will collide with a horizontal wall (-1 if no collision)
     *
     *  The wall is assumed to be infinitely wide (which is OK as the
     *  game board is enclosed anyways)
     *
     *  @param ball the ball
     *  @param wallY the Y coordinate of the ball */
	public float? timeUntilHorizontalWallCollision(Ball ball, float wallY)
	{
		if (ball.velocity.Y == 0f)
		{
			return null;
		}
		else
		{
			var t = MathF.Min((wallY - ball.radius - ball.location.Y) / ball.velocity.Y,
				(wallY + ball.radius - ball.location.Y) / ball.velocity.Y);
			if (t < 0f)
			{
				return null;
			}
			else
			{
				return t;
			}
		}
	}

	/** Returns the time until the ball will be pocketed */
	public float? timeUntilPocketed(Ball ball)
	{
		float? foundTime = null;

		foreach (Pocket pocket in Pockets)
		{
			var curTime = timeUntilCollision(ball, pocket);
			if (foundTime.HasValue
				&& curTime.HasValue
				&& curTime < foundTime)
			//if (curTime.exists(ct => foundTime.forall(ct < _)))
			{
				foundTime = curTime;
			}
		}

		return foundTime;
	}

	/** Returns the time when the ball will collide with a vertical wall (-1 if no collision)
     *
     *  The wall is assumed to be infinitely tall (which is OK as the
     *  game board is enclosed anyways)
     *
     *  @param ball the ball
     *  @param wallX the X coordinate of the ball */
	public float? timeUntilVerticalWallCollision(Ball ball, float wallX)
	{
		if (ball.velocity.X == 0f)
		{
			return null;
		}
		else
		{
			var t = MathF.Min((wallX - ball.radius - ball.location.X) / ball.velocity.X,
				(wallX + ball.radius - ball.location.X) / ball.velocity.X);
			if (t < 0f)
			{
				return null;
			}
			else
			{
				return t;
			}
		}
	}

	/** Updates the position, velocity, and angular velocity of the balls in the game state
      *
      *  Call once every time step
      *
      *  @param state the state to update
      *  @param t the duration of the time step
      */
	public void update(IEnumerable<Ball> balls, float t)
	{

		//Update the velocities of the balls
		updateVelocities(balls, t);
	}

	/** Find all collisions that will occur within the time step and update the position and velocities accordingly
          *
          * @param rt The amount of time, in seconds, that remains of this time step
          * @param depth How many times we have recursed so far (will abort at 100)
          */
	public void applyCollisionsRecursive(List<Ball> balls, float rt, int depth = 0)
	{

		var (timeUntilCollision, collisions) = getNextCollisions(balls);

		if (timeUntilCollision < rt)
		{

			//All the velocity updates that will be applied to a ball during this time step
			//var newVelocity = Map[Ball, Vector[Vector3]]().withDefaultValue(Vector[Vector3]());
			//var newAngularVelocity = Map[Ball, Vector[Vector3]]().withDefaultValue(Vector[Vector3]());

            var newVelocity = new Dictionary<Ball, List<Vector3>>();
            var newAngularVelocity = new Dictionary<Ball, List<Vector3>>();

			collisions.ForEach(c =>
			{
				// collisions foreach { case (collisionType, ball1, oBall2) => collisionType match {
				switch (c.Type)
				{
					case CollisionType.BallBall: {
						    var result = collide(c.Ball1, c.Ball2!);

						    newVelocity += ball1->(newVelocity(ball1) :+result._1.velocity);
						    newVelocity += ball2->(newVelocity(ball2) :+result._2.velocity);

						    newAngularVelocity += ball1->(newAngularVelocity(ball1) :+result._1.angularVelocity);
						    newAngularVelocity += ball2->(newAngularVelocity(ball2) :+result._2.angularVelocity);
                            break;
                        }
					case CollisionType.HorizontalWall:
                    {
							var result = collideWall(ball1, true);
							newVelocity += ball1->(newVelocity(ball1) :+result.velocity);
							newAngularVelocity += ball1->(newVelocity(ball1) :+result.angularVelocity);
							break;
						}

					case CollisionType.VerticalBall:
                    {
							var result = collideWall(ball1, false);
							newVelocity += ball1->(newVelocity(ball1) :+result.velocity);
							newAngularVelocity += ball1->(newVelocity(ball1) :+result.angularVelocity);
							break;
						}

					//Remove pocketed balls
                    case CollisionType.Pocketed:
                    {
                        //state.removeBall(ball1);
						break;
                    }
				}
			});

			//Move the balls to collision positions before updating the velocities
			// timeUntilCollision foreach { ct => { moveBalls(balls, 0.99f * ct) } }
			
            moveBalls(balls, 0.99f * timeUntilCollision.Value);

			//Update the velocity by adding the average change in velocity
			newVelocity foreach { case (ball, dVelocities) =>
				{
					var p = (1f / dVelocities.size);
					ball.velocity += p * dVelocities.foldLeft(new Vector3(0f, 0f, 0f))(_ + _);
				}

			}

			//Update the velocity by adding the average change in angular velocity
			newAngularVelocity foreach { case (ball, dAVelocities) =>
				{
					var p = 1f / dAVelocities.size;
					ball.angularVelocity += p * dAVelocities.foldLeft(new Vector3(0f, 0f, 0f))(_ + _);
				}

			}

			//Loop at most 100 times per time step
			if (depth < 100)
			{
				applyCollisionsRecursive(balls, rt - timeUntilCollision.Value, depth + 1);
			}

		}
		else
		{
			moveBalls(balls, rt);
		}


		applyCollisionsRecursive(t);

	}

	/** Updates the velocities of the given balls
      *
      *  The logic is based on the equations from the following link:
      *  http://archive.ncsa.illinois.edu/Classes/MATH198/townsend/math.html
      *
      *  @param balls the balls to update
      *  @param t time since last execution (in seconds) */
	public void updateVelocities(IEnumerable<Ball> balls, float t)
	{
		foreach (Ball ball in balls)
		{

			{

				/* Determine if the ball is rolling or not, this is
                 * deemed to be the case if the relative velocity
                 * between the edge of the ball and the table is
                 * almost zero. */

				if (getRelativeVelocity(ball).Length() <= 0.02f)
				{

					//Calculate the new velocity according to ∆v = -µg (v/|v|) ∆t
					var newVelocity = ball.velocity + (-cfr * 9.8f * ball.velocity.normalized() * t);

					//If both the velocity and the angular velocity are almost zero, stop the ball completely
					if (ball.velocity.Length() < 0.01f && ball.angularVelocity.Length() < 0.01f)
					{
						ball.velocity = new Vector3(0f, 0f, 0f);
						ball.angularVelocity = new Vector3(0f, 0f, 0f);
					}
					else
					{

						//Otherwise update the velocities
						ball.velocity = newVelocity;

						//As the ball is rolling, the angular velocity should equal the velocity
						ball.angularVelocity =
							new Vector3(-newVelocity.Y / ball.radius, newVelocity.X / ball.radius, 0);
					}
				}
				else
				{

					//--Sliding ball--
					var pv = getRelativeVelocity(ball).normalized();

					//Calculate the new velocity according to ∆v = -µg (v/|v|) ∆t
					ball.velocity += -cfs * 9.8f * pv * t;

					/* Calculate the new angular velocity using ∆ω = 5/2 (R X (-µmgr (v/|v|))) ∆t/(m*r^2)
                     * where R is a vector pointing from the middle of the ball to the touching point
                     * with the table (0,0,-r) */
					ball.angularVelocity += (5f * t / (2f * ball.mass * MathF.Pow(ball.radius, 2)))
											* (Vector3.Cross(new Vector3(0f, 0f, -ball.radius),
												(-cfs * ball.mass * g * ball.radius * pv)));
				}
			}

		}

	}
}