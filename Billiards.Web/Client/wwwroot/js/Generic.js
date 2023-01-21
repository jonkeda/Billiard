window.getWindowDimensions = function () {
    var orientation = 0;
    try {
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
    }
    catch (ex) {
        orientation = 1;
    }
    return {
        width: window.innerWidth,
        height: window.innerHeight,
        orientation: orientation
    };
};


window.openFullscreen = function () {
    try {
        var elem = document.documentElement;
        if (elem.requestFullscreen) {
            elem.requestFullscreen();
        } else if (elem.mozRequestFullScreen) { /* Firefox */
            elem.mozRequestFullScreen();
        } else if (elem.webkitRequestFullscreen) { /* Chrome, Safari and Opera */
            elem.webkitRequestFullscreen();
        } else if (elem.msRequestFullscreen) { /* IE/Edge */
            elem.msRequestFullscreen();
        } else {
            return false;
        }
        return true;
    }
    catch (ex) {
        return true;
    }
}


window.closeFullscreen = function () {
    if (document.exitFullscreen) {
        document.exitFullscreen();
    } else if (document.mozCancelFullScreen) { /* Firefox */
        document.mozCancelFullScreen();
    } else if (document.webkitExitFullscreen) { /* Chrome, Safari and Opera */
        document.webkitExitFullscreen();
    } else if (document.msExitFullscreen) { /* IE/Edge */
        document.msExitFullscreen();
    }
    else {
        return false;
    }
    return true;
}