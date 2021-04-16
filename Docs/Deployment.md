# Deployments

Follow the general company standards with [GitFlow](../.github/workflows/gitflow.yml) and changelog generated pipeline:

1. Create a **release branch** for the release number (e.g. _release/2.9.11_) and push it to the origin.

2. Perform the adjustments on the branch if needed e.g. last fixes related with the **release**.

3. Add an **annotated tag** with the release number (e.g. _2.9.11_) on the release branch and push tag to the origin.

4. The above step should trigger the [**automation on OUR Github**](https://github.com/thecogworks/Cogworks.Examine.Tweaks/actions) and regenerate changelog + move the tag to the master branch.

5. After that it will create and publish NuGet package for feeds: NuGet, GitHub Packages, MyGet (optionally - can be disabled in secure variables), Our.Umbraco (just created zip file)
