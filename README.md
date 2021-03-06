<!-- <p align="center">
  <a href="" rel="noopener">
  <img width="200px" height="200px" src="Docs/img/logo.jpg" alt="Project logo"></a>
</p> -->

<h3 align="center">Cogworks Examine Tweaks</h3>

<div align="center">

[![Project Code](https://img.shields.io/static/v1?label=&message=Cogworks.Examine.Tweaks&color=lightgray&style=flat-square)]() [![Version](https://img.shields.io/static/v1?label=&message=version&color=informational&style=flat-square)](https://github.com/thecogworks/Cogworks.Examine.Tweaks/releases) [![License](https://img.shields.io/badge/license-MIT-4c9182.svg)](LICENSE.md)

</div>

---

<details open="open">
<summary>Table of Contents</summary>

- [About](#about)
- [Getting Started](#getting_started)
- [Deployment](#deployment)
- [Usage](#usage)
- [Changelog](#changelog)
- [Built Using](#built_using)

</details>

## About <a name = "about"></a>

Set of the Umbraco Examine tweaks and hacks.

## Getting Started <a name = "getting_started"></a>

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

See instructions for [Backend](Source/README.md) setup.

See [deployment](#deployment) for notes on how to deploy the project on a live system.

## Usage <a name="usage"></a>

In Web.config set those configurations:

- Enabling package (default false):


```xml
<add key="Cogworks.Examine.Tweaks.Enabled" value="true" />
```

- Enabling custom **PublishedContent** value set builder (default is true):


```xml
<add key="Cogworks.Examine.Tweaks.UsePublishedContentCustomValueSetBuilder" value="true" />
```

- Enabling custom **Content** value set builder (default is true):


```xml
<add key="Cogworks.Examine.Tweaks.UseContentCustomValueSetBuilder" value="true" />
```

- Disabling **Internal** index (default false):


```xml
<add key="Cogworks.Examine.Tweaks.InternalIndexDisabled" value="true" />
```

- Disabling **External** index (default false):


```xml
<add key="Cogworks.Examine.Tweaks.ExternalIndexDisabled" value="true" />
```

- Internal included item types comma separated (if not in use remove setting - not empty value)


```xml
<add key="Cogworks.Examine.Tweaks.InternalIncludedItemTypes" value="testPage,anotherTestPage" />
```

- Internal excluded item types comma separated (if not in use remove setting - not empty value)


```xml
<add key="Cogworks.Examine.Tweaks.InternalExcludedItemTypes" value="testPage,anotherTestPage" />
```

- External included item types comma separated (if not in use remove setting - not empty value)


```xml
<add key="Cogworks.Examine.Tweaks.ExternalIncludedItemTypes" value="testPage,anotherTestPage" />
```

- External excluded item types comma separated (if not in use remove setting - not empty value)


```xml
<add key="Cogworks.Examine.Tweaks.ExternalExcludedItemTypes" value="testPage,anotherTestPage" />
```

## Deployment <a name = "deployment"></a>

All the information required for the proper project deployments are located inside of the [Deployment.md](Docs/Deployment.md) file in the [Docs](Docs/) directory. Please go there if you're about to perform any new release of the project.

## Changelog <a name = "changelog"></a>

All notable changes to this project can be found in [CHANGELOG.md](CHANGELOG.md).

## Built Using <a name = "built_using"></a>

- [Umbraco](https://umbraco.com/) - CMS
- [GitHub Actions](https://docs.github.com/en/free-pro-team@latest/actions) - DevOps and changelog generator

