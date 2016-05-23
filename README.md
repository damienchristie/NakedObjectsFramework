NakedObjectsFramework
=====================

The Master branch of this repository contains source for version 8 of the Naked Objects Framework (NOF8). Though this is currently at Beta stage, it is very usable, and, if you are new to Naked Objects we recommend starting with NOF8 rather than the older, stable, NOF7.

Note that both NOF7 and NOF8 can be used as packages from the NuGet public gallery.  There is no need to clone this repository.  Indeed, building the framework from source code is quite complex and not recommended for newcomers.  (If you really want to know how to do it, see the section 'How to build the framework from source code' in the Developer Manual.)

NOF8
====

NOF8, currently at Beta stage, introduces a radically different user interface based on the Single Page Application (SPA) architecture. It uses identical domain model programming conventions as NOF7; indeed it is possible to run the NOF8 client and the NOF7 client alongside each other, as two different 'run' projects talking to the same domain model project(s).

The best way to try NOF8 is to run the NakedObjects.Template application, which may be downloaded as a .zip file from: https://github.com/NakedObjectsGroup/NakedObjectsFramework/blob/master/Run/NakedObjects.Template.zip?raw=true 

(If the unzipped application does not run first time, please see the developer manual for further hints).

The developer manual for NOF 8 is available here: .
https://github.com/NakedObjectsGroup/NakedObjectsFramework/blob/master/Documentation/DeveloperManual.docx?raw=true

When searching the NuGet package gallery for new releases of NOF8 (which are quite frequent at present) please ensure you have the 'include pre-releases' checkbox selected.

NOF8 source code is held in the master branch of this repository.

NOF7
====

NOF7 is the current stable release.  However, since it will soon be superseded by NOF8, we recommend that newcomers to Naked Objects start working with the NOF8 beta release from the outset.

The NOF7 client is built upon the ASP.NET MVC 5 framework.  (This will be replaced by an entirely new client architecture in NOF8, but with no changes required to your domain model code).

NOF7 packages are available on the NuGet public gallery as stable releases.

NOF7 source code is available on the 7.0 branch of this repository.

The developer manual for NOF7 may be downloaded from here:
https://github.com/NakedObjectsGroup/NakedObjectsFramework/blob/7.0/Documentation/DeveloperManual.docx?raw=true


