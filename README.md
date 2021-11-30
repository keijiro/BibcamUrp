URP Custom Renderer Feature for Bibcam
--------------------------------------

This repository contains a URP custom [renderer feature] that adds URP support
to [Bibcam].

I implemented the feature in a single script file --
[BibcamBackgroundPassFeature.cs]. Copy it into your project and enable it in
the [renderer settings]. Then the background drawer works as on the default
built-in render pipeline.

[BibcamBackgroundPassFeature.cs]:
  /Assets/BibcamBackgroundPassFeature.cs

[renderer feature]:
  https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@13.1/manual/urp-renderer-feature.html

[Bibcam]:
  https://github.com/keijiro/Bibcam

[renderer settings]:
  https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@13.1/manual/urp-renderer-feature-how-to-add.html
