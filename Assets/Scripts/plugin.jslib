mergeInto(LibraryManager.library, {
    SetFinalScore: function(points) {
        var event = new CustomEvent("gameProgress", {
            detail: { score: points },
        });
        window.dispatchEvent(event);
    },
    SetViewed: function() {
        window.parent.mod_webgl_plugin.trackGameViewed()
    }
});