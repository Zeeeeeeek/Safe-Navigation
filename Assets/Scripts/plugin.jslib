mergeInto(LibraryManager.library, {
    SetFinalScore: function(points) {
        var event = new CustomEvent("gameProgress", {
            detail: { score: points },
        });
        window.dispatchEvent(event);
    }
});