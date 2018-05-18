# LocalizedLanguageManagerForUnity
This component is a Language Localization for Unity.
For Android only.

1. This component read a file contains localized language data from StreamingAssets folder,installed folder to Android or user manually inputed path.
2. Loaded data are devided a pair of key and value, stored to Dictinary type data.
3. When you need localized language data, you call GetLocalizedString(string key).

A format of localize language data must be "key=value".

You must make "eng.llf", this is a default file.(if you use SetFileExtension() or SetDefaultLanguage(),you must change to the filename of the changed-file.)

if you use streamingAsset-mode, you must put llf files into StreamingAssets folder. 

persistentData-mode and manuallyInput-mode are for using a user-defined language.
This is a mechanism for users to prepare languages for which you can not prepare translations.
