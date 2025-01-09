mergeInto(LibraryManager.library, {

  SetScore: function (score) {
    console.log("Setting score: ", score);
    console.log("Score type ", typeof score);
    const progressData = {score: score};
    window.parent.mod_webgl_plugin.trackGameProgress(progressData);
  },
});