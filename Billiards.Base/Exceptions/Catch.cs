namespace Billiards.Base.Exceptions
{
    public static class Catch
    {
        public static void All(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch 
            {
                // fail
            }
        }

        public static void ShowMessageBox(Action action)
        {
            // action.Invoke();
            try
            {
                action.Invoke();
            }
            catch (Exception e)
                {
//                ThreadDispatcher.Invoke(() =>  MessageBox.Show(e.Message));
            }
        }

        public static T Return<T>(Func<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch
            {
                return default(T);
                // fail
            }
        }
    }
}
