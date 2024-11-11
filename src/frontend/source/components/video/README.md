# Video component

## Table of contents

1. [What does it do](#markdown-header-what-does-it-do)
2. [Install](#markdown-header-install)
3. [How to use](#markdown-header-how-to-use)
4. [Dependencies](#markdown-header-dependencies)
5. [Changelog](#markdown-header-changelog)
6. [Developers](#markdown-header-developers)

![Video Demo](https://media.giphy.com/media/7AaqrqQUPys39wn1CN/giphy.gif)

## What does it do

- Plays native video
- Fires generic video ready, play & pause events.

## Install

Import module

```javascript
import '@/utilities/in-view'
import '@/components/image'
import { videoLoader } from '@/components/video'
```

## How to use

### Default

```javascript
import '@/utilities/in-view'
import '@/components/image'
import { videoLoader } from '@/components/video'

videoLoader(['native'])
  .then(([platforms, Video]) => {
    Video.registerPlatforms(platforms)
  })
  .catch(() => null)
```

### With nullcheck

If you want to create a nullCheck in case there is no video element on the page:

```javascript
if (document.querySelector('[js-hook-video]')) {
  videoLoader(['native'])
    .then(([platforms, Video]) => {
      Video.registerPlatforms(platforms)
    })
    .catch(() => {})
}
```

The promise in VideoLoader will throw an error that ends up in your catch, if no video element is found.

Create player in HTML. The player will use the [in-view library](/utilities/in-view/) to initialise the videos when they're in view.

```htmlmixed
{% from 'video.html' import video  %}

{{ video({
    instance_id: 1,
    id: 'GrDHJK9UYpU',
    platform: 'youtube',
    title: 'title here',
    description: 'description here',
    thumbnail: '/assets/images/thumbs/thumb.jpg',
    total_time: 'T1M33S',
    start_time: '10',
    classes: 'additional-class',
    controls: true,
    image: {
      backgroundColor: '#8894ae'
    }
}) }}
```

### Without in-view

This will initialise all the players on the page. If autoplay parameter is set, it will also autoplay all videos.

```javascript
import '@/components/image'
import { videoLoader } from '@/components/video'

videoLoader(['native'])
  .then(([platforms, Video]) => {
    Video.registerPlatforms(platforms)

    Events.$trigger('video::update')
  })
  .catch(() => {})
```

Create the player the same as in the previous demo. But now add a `inview: false` as parameter.

```htmlmixed
{% from 'video.html' import video  %}

{{ video({
    instance_id: 1,
    id: 'GrDHJK9UYpU',
    platform: 'youtube',
    title: 'title here',
    description: 'description here',
    thumbnail: '/assets/images/thumbs/thumb.jpg',
    total_time: 'T1M33S',
    start_time: '10',
    classes: 'additional-class',
    controls: true,
    inview: false,
    image: {
      backgroundColor: '#8894ae'
    }
}) }}
```

### Native video

You can initialise native video elements with srcset detect, it will pick the closest source based on you screen size and the available source sizes.

```javascript
import '@/utilities/in-view'
import '@/components/image'
import { videoLoader } from '@/components/video'

videoLoader(['native'])
  .then(([platforms, Video]) => {
    Video.registerPlatforms(platforms)

    Events.$trigger('video::update')
  })
  .catch(() => {})
```

```htmlmixed
{% from 'video.html' import video  %}

{{ video({
    instance_id: 1,
    id: '1',
    platform: 'native',
    title: 'title here',
    description: 'description here',
    thumbnail: '/assets/images/thumbs/thumb.jpg',
    total_time: 'T1M33S',
    start_time: '10',
    classes: 'additional-class',
    controls: true,
    closedcaptions: [
        {
            url: 'url to vtt file',
            label: 'Nederlands',
            lang: 'nl'
        }
    ],
    sources: [
        {
            size : 1920,
            source: [
                {
                    url: 'https://temp.media/video/?width=1920&height=1080&length=10',
                    type: 'video/mp4'
                }
            ]
        },
        {
            size : 1280,
            source: [
                {
                    url: 'https://temp.media/video/?width=1280&height=720&length=10',
                    type: 'video/mp4'
                }
            ]
        },
        {
            size : 1024,
            source: [
                {
                    url: 'https://temp.media/video/?width=1024&height=576&length=10',
                    type: 'video/mp4'
                }
            ]
        },
        {
            size : 320,
            source: [
                {
                    url: 'https://temp.media/video/?width=480&height=270&length=10',
                    type: 'video/mp4'
                }
            ]
        }
    ]
}) }}
```

## Dependencies

- [Image component](/components/image/)
- [Events utility](/utilities/events/)
- [In-view utility](/utilities/in-view/)

## Developers

- [Adrian Klingen](mailto:adrian.klingen@deptagency.com)
