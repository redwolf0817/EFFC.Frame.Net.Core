; (function (fns) {
    fns.Config.Exception =
        {
            beforeProcess: function () {
                if (CloseProcessDiv) {
                    CloseProcessDiv();
                }
            }
        }
}(FrameNameSpace));
