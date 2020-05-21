function startup() {

    var windowOptions = {
        width: 500,
        height: 375,
        show: false,
    };

    workbench.view.initSplashScreen({
        windowOpts: windowOptions,
        templateUrl: "assets/images/logo.png",
        delay: 0,
        minVisible: 1500,
        splashScreenOpts: {
            height: 500,
            width: 500,
            transparent: true,
        },
    });
}


