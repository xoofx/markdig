anchors.add();
var jstoc = document.getElementsByClassName("js-toc");
if (jstoc.length > 0)
{
    tocbot.init({
        // Which headings to grab inside of the contentSelector element.
        headingSelector: 'h2, h3, h4, h5',
        collapseDepth: 3,
        orderedList: true,
        hasInnerContainers: true,
    });
}

(function () {
  const InitLunetTheme = function (e) {
    if (halfmoon.darkModeOn != e.matches) {
      halfmoon.toggleDarkMode();
    }
  }

  let colorSchemeQueryList = window.matchMedia('(prefers-color-scheme: dark)')
  InitLunetTheme(colorSchemeQueryList);
  colorSchemeQueryList.addListener(InitLunetTheme);
})();
