window.getWindowDimensions = function () {
    var orientation = 0;
    switch (screen.orientation.type) {
/*    case "landscape-primary":
    case "landscape-secondary":
            orientation = "Landscape";
        break;
*/
    case "portrait-secondary":
    case "portrait-primary":
        orientation = 1;
        break;
    }
    return {
        width: window.innerWidth,
        height: window.innerHeight,
        orientation: orientation
    };
};