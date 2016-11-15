using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Lynicon")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Lynicon")]
[assembly: AssemblyCopyright("Copyright ©  2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("51825df5-efa9-4895-a1e6-abd9e62c4fd4")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// Web Resources
[assembly: WebResource("Lynicon.Scripts.LyniconMain.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.LyniconControls.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.LyniconEditPanel.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.jquery.jstree.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.jquery.jstreelist.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.jquery.layout.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.jquery-ui.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.fileuploader.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.jquery.tmpl.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.jquery.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.jquery.contextMenu.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.jquery.simplemodal.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.BrowserDetect.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.chosen.jquery.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.jquery.hopscotch.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.masonry.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.jquery-te.js", "application/javascript")]
[assembly: WebResource("Lynicon.Scripts.selectize.js", "application/javascript")]


[assembly: WebResource("Lynicon.Content.LyniconMain.css", "text/css")]
[assembly: WebResource("Lynicon.Content.jquery.jstreelist.css", "text/css")]
[assembly: WebResource("Lynicon.Content.jquery.layout.css", "text/css")]
[assembly: WebResource("Lynicon.Content.fileuploader.css", "text/css")]
[assembly: WebResource("Lynicon.Scripts.themes.default.style.css", "text/css")]
[assembly: WebResource("Lynicon.Content.jquery.contextMenu.css", "text/css")]
[assembly: WebResource("Lynicon.Content.jquery-ui.css", "text/css")]
[assembly: WebResource("Lynicon.Content.jquery-te.css", "text/css")]
[assembly: WebResource("Lynicon.Content.L24External.css", "text/css")]
[assembly: WebResource("Lynicon.Content.chosen.css", "text/css")]
[assembly: WebResource("Lynicon.Content.selectize.css", "text/css")]

[assembly: WebResource("Lynicon.Views.TI.aspx", "text/html")]

[assembly: WebResource("Lynicon.Content.Images.application.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.close-white.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.code.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.css.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.cut.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.db.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.directory.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.doc.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.door.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.dropdownarrow.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.file.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.film.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.flash.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.folder_open.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.html.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.java.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.jquery-te.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.linux.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.loading.gif", "image/gif")]
[assembly: WebResource("Lynicon.Content.Images.music.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.page_white_copy.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.page_white_delete.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.page_white_edit.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.page_white_paste.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.pdf.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.php.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.picture.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ppt.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.psd.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ruby.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.script.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.spinner.gif", "image/gif")]
[assembly: WebResource("Lynicon.Content.Images.txt.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-bg_diagonals-thick_18_b81900_40x40.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-bg_diagonals-thick_20_666666_40x40.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-bg_flat_10_000000_40x100.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-bg_glass_65_ffffff_1x400.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-bg_glass_100_f6f6f6_1x400.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-bg_glass_100_fdf5ce_1x400.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-bg_gloss-wave_35_f6a828_500x100.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-bg_highlight-soft_75_ffe45c_1x100.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-bg_highlight-soft_100_eeeeee_1x100.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-icons_228ef1_256x240.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-icons_222222_256x240.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-icons_ef8c08_256x240.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-icons_ffd27a_256x240.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.ui-icons_ffffff_256x240.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.xls.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.zip.png", "image/png")]
[assembly: WebResource("Lynicon.Content.Images.chosen-sprite.png", "image/png")]
[assembly: WebResource("Lynicon.Scripts.themes.default.d.png", "image/png")]
[assembly: WebResource("Lynicon.Scripts.themes.default.d.gif", "image/gif")]
[assembly: WebResource("Lynicon.Scripts.themes.default.throbber.gif", "image/gif")]
