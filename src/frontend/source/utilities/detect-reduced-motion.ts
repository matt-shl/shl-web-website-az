const IS_REDUCED_MOTION_CLASS = 'is--reduced-motion'
const html = document.documentElement

class DetectReducedMotion {
  constructor() {
    const isReduced = window.matchMedia(`(prefers-reduced-motion: reduce)`).matches === true
    const action = isReduced ? 'add' : 'remove'

    html.classList[action](IS_REDUCED_MOTION_CLASS)
  }
}

export default new DetectReducedMotion()
