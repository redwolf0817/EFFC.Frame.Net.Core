require.config({
    paths: {
        jQuery: "/scripts/jquery-1.11.3.min",
        effc: "/scripts/FrameJs/frame",
        net: "/scripts/FrameJs/frame.net",
        exception: "/scripts/FrameJs/frame.exception",
        promise: "/scripts/FrameJs/frame.promises",
        extentions: "/scripts/FrameJs/frame.extention",
        message: "/scripts/FrameJs/frame.message",
        ajaxconfig: "/scripts/FrameJs/configs/ajax.config",
        messageconfig:"/scripts/FrameJs/configs/message.config",
        app:"/js/app"
    },
    shim: {
        jQuery: {
            exports:'jQuery'
        },
        promise: ['effc'],
        message: ['jQuery', 'effc','messageconfig'],
        exception: ['effc'],
        extentions: ['effc'],
        net: ['effc', 'message', 'exception','ajaxconfig']
    }
})
require(['app'])