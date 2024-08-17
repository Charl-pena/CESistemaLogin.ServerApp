function clearUri(newPath) {
   window.history.replaceState({}, document.title, newPath);
}