define(['effc'],function (fns) {
    fns.ExceptionProcess = {
        Process : function (error, sourcename) {
            if (error != null) {
                console.error("An error occured in \"" + sourcename + "\"!\n" + (error.message ? error.message : error));
            }
        }
    }
});

