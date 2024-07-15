'use strict'

/*------------------------------------*\
 * JS Main entry file
\*------------------------------------*/
import './config'
import '@/utilities/detect-touch'
import '@/utilities/detect-reduced-motion'
import '@/utilities/detect-keyboard-focus'
import '@/utilities/set-theme'
import '@/components/image'
import '@/utilities/focus-trap'

import moduleInit from '@/utilities/module-init'

moduleInit.async('[js-hook-grid-overlay]', () => import('@/components/grid-overlay'))
moduleInit.async('[js-hook-modal]', () => import('@/components/modal'))
moduleInit.async('[js-hook-accordion]', () => import('@/components/accordion'))
moduleInit.async('[js-hook-toast]', () => import('@/components/toast'))
moduleInit.async('[js-hook-slogan]', () => import('@components/slogan'))
moduleInit.async('[js-hook-header]', () => import('@components/header'))
moduleInit.async('[js-hook-navigation-desktop]', () => import('@components/navigation-desktop'))
