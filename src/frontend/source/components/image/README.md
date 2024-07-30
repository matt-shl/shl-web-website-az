# Image component

## Table of contents

1. [What does it do](#markdown-header-what-does-it-do)
2. [Install](#markdown-header-install)
3. [How to use](#markdown-header-how-to-use)
4. [Dependencies](#markdown-header-dependencies)
5. [Developers](#markdown-header-developers)

## What does it do

- Lazyloads images when in view
- Generates placeholder images per breakpoint
- Shows skeleton whilst loading

## Install

Import module

```javascript
import '@/components/image'
```

## How to use

**The updated image component `version 2.0.0+` will render a `<picture />` element to allow for more granular control over images and different crops per breakpoint. This behavior will also be adapted in the new Umbraco 9 standard.**

| property               | required | value                                      |
| ---------------------- | -------- | ------------------------------------------ |
| `classes`              | no       | `string`                                   |
| `backgroundColor`      | no       | `string`                                   |
| `aspect`               | no       | `number[]`                                 |
| `hidden`               | no       | `boolean`                                  |
| `objectFit`            | no       | `boolean`                                  |
| `attr`                 | no       | `string`                                   |
| `caption`              | no       | `string`                                   |
| `alt`                  | yes      | `string`                                   |
| `imageStyle`           | no       | `type of data/images/styles/${style}.json` |
| `image`                | no       | `string`                                   |
| `fetchPriority`        | no       | `'high' | 'low' | 'auto'`                  |
| `preload` _deprecated_ | no       | `string`                                   |
| `srcset` _deprecated_  | no       | `string`                                   |
### Default

By default the image component will always auto-generate placeholder images by using https://satyr.dev.
If you do not wish for this behavior please see the section: [custom image sources](#markdown-header-custom-image-sources) on how to override this.

```javascript
import '@/components/image'
```

```htmlmixed
{% from 'image.html' import image  %}

{{ image({
  backgroundColor: '#8894ae' // Can be used in FE to load a custom colored placeholder image in backend this can be determined by the prominent image color
}) }}
```

### Custom image crops

The default image crops are defined [here](/source/data/images/styles/default.json)
The default image breakpoints and pixel densities are defined [here](/source/data/images/breakpoints.json)

You can add new styles by adding them to the styles [folder](/source/data/images/styles/) to easily create and reuse styles across components.

Style generation works as followed:

#### Image crop with fixed sizes

Filename: `card.json`

```javascript
{
  "aspect": [
    4,
    3
  ],
  "breakpoints": {
    "mobile": [
      480, // width
      270 // height
    ],
    "mobilePlus": [
      768,
      432
    ],
    "tabletPortrait": [
      464,
      261
    ],
    "tabletLandscape": [
      375,
      281
    ],
    "laptop": [
      500,
      375
    ]
  }
}
```

```htmlmixed
{% from 'image.html' import image  %}

{{ image({
  imageStyle: 'card',
  backgroundColor: '#8894ae'
}) }}
```

#### Image crop with fixed dynamic height

Filename: `wysiwyg.json`

```javascript
{
  "breakpoints": {
    "mobile": [
      480, // width
      0 // height
    ],
    "mobilePlus": [
      768,
      0
    ],
    "tabletPortrait": [
      464,
      0
    ],
    "tabletLandscape": [
      375,
      0
    ],
    "laptop": [
      500,
      0
    ]
  }
}
```

```htmlmixed
{% from 'image.html' import image  %}

{{ image({
  imageStyle: 'wysiwyg',
  aspect: [16, 9], // This is needed in FE to be able to generate a placeholder image (backend will generate it dynamically)
  backgroundColor: '#8894ae'
}) }}
```

### Custom image sources

Below is an example on how to use custom images instead of the auto-generated placeholder images.
These sources breakpoints should still match your `imageStyle` breakpoints.

```htmlmixed
{% from 'image.html' import image  %}

{{ image({
  imageStyle: 'card',
  sources: {
    "mobile": '/assets/images/tmp/placeholder.400x250.jpg 1x',
    "mobilePlus": '/assets/images/tmp/placeholder.400x250.jpg 1x',
    "tabletPortrait": '/assets/images/tmp/placeholder.400x250.jpg 1x',
    "tabletLandscape": '/assets/images/tmp/placeholder.400x250.jpg 1x',
    "laptop": '/assets/images/tmp/placeholder.400x250.jpg'
  }
}) }}
```

### Images with fetch priority
Images loaded with the fetchPriority attribute set are not lazy loaded and instruct the browser to load the image with the appropriate priority. This helps reducing the Largest Contentful Paint on page load.

This can be used in example for Hero components that are rendered above the page fold on page load.

More information can be found in the [MDN docs](https://developer.mozilla.org/en-US/docs/Web/API/HTMLImageElement/fetchPriority)



## Dependencies

- In-view libary
- Events libary

## Developers

- [Adrian Klingen](mailto:adrian.klingen@deptagency.com)
- [Mark Smits (co author)](mailto:mark.smits@deptagency.com)

