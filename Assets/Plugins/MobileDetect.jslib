mergeInto(LibraryManager.library, {
  IsMobileBrowser: function () {
    // Primary check: touch-primary device (coarse pointer = finger, not mouse)
    var coarse = window.matchMedia && window.matchMedia('(pointer: coarse)').matches;
    // Secondary: common mobile user agents as fallback
    var ua = /Android|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    return coarse || ua;
  }
});
