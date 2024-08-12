'use strict'

/*------------------------------------*\
 * JS Main entry file
\*------------------------------------*/
import './config'
import '@/utilities/detect-touch'
import '@/utilities/detect-reduced-motion'
import '@/utilities/detect-keyboard-focus'
import '@/utilities/set-theme'
import '@/utilities/set-language'
import '@/utilities/replace-content'
import '@/utilities/in-view'
import '@/components/image'
import '@/utilities/focus-trap'
import '@/utilities/scroll-to'
import '@/components/loading-indicator'

import Events from '@utilities/events'

import {videoLoader} from '@/components/video'
import moduleInit from '@/utilities/module-init'

if (document.querySelector('[js-hook-page-load-animation-trigger]')) {
  import('@/utilities/page-load-animation')
}

moduleInit.async('[js-hook-grid-overlay]', () => import('@/components/grid-overlay'))
moduleInit.async('[js-hook-modal]', () => import('@/components/modal'))
moduleInit.async('[js-hook-accordion]', () => import('@/components/accordion'))
moduleInit.async('[js-hook-toast]', () => import('@/components/toast'))
moduleInit.async('[js-hook-slogan]', () => import('@components/slogan'))
moduleInit.async('[js-hook-header]', () => import('@components/header'))
moduleInit.async('[js-hook-navigation-desktop]', () => import('@components/navigation-desktop'))
moduleInit.async('[js-hook-flyout]', () => import('@components/flyout'))
moduleInit.async('[js-hook-anchor-list]', () => import('@components/anchor-list'))
moduleInit.async('[js-hook-carousel]', () => import('@/components/carousel'))
moduleInit.async('[js-hook-filters]', () => import('@components/filters'))
moduleInit.async('[js-hook-rich-text]', () => import('@/components/rich-text'))
moduleInit.async('[js-hook-banner-quote]', () => import('@components/banner-quote'))
moduleInit.async('[js-hook-carousel-indicator]', () => import('@components/carousel-indicator'))
moduleInit.async('[js-hook-form]', () => import('@/components/form'))

if (document.querySelector('[js-hook-video]')) {
  videoLoader(['native'])
    .then(([platforms, Video]) => {
      Video.registerPlatforms(platforms)
      Events.$trigger('video::update')
    })
    .catch(() => {
      console.warn('No video platforms found.')
    })
}
