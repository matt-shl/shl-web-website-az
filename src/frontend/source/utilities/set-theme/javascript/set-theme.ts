import Environment from '@utilities/environment'

const themes = [
  'general',
  'pale-blue',
  'lightest-blue',
  'pastel-blue',
  'light-blue',
  'lightest-pink',
  'white-pink',
  'pale-pink',
  'dark-pink',
  'pale-green',
  'light-grey',
  'dark-green',
  'pastel-green',
  'pale-yellow',
  'lightest-yellow',
  'white',
] as const

type Theme = (typeof themes)[number]

const JS_HOOK_THEME_LABEL = '[js-hook-current-theme]'
const THEME_PREFIX = 't-'
const SWITCH_THEME_DELAY_IN_MS = 300

class SetTheme {
  html: HTMLElement
  currentTheme: Theme
  isSwitching: boolean
  allThemedSections: HTMLElement[]
  themeLabel: HTMLSpanElement | null

  constructor() {
    if (!Environment.isLocal && !Environment.isTest) return

    this.html = document.documentElement
    this.allThemedSections = Array.from(document.querySelectorAll('.c-layout-section'))
    this.currentTheme = this.getCurrentTheme(this.html)

    this.themeLabel = document.querySelector(JS_HOOK_THEME_LABEL)

    this.bindEvents()
  }

  bindEvents() {
    document.addEventListener('keydown', event => {
      if (!this.isInputElement(event.target as HTMLElement)) {
        if (event.shiftKey && event.key === 'T') {
          if (this.isSwitching) return
          this.switchTheme(this.html)
          this.isSwitching = true
          setTimeout(() => (this.isSwitching = false), SWITCH_THEME_DELAY_IN_MS)
        } else if (event.ctrlKey && event.key === 't') {
          if (this.isSwitching) return
          this.switchThemeSection()
          this.isSwitching = true
          setTimeout(() => (this.isSwitching = false), SWITCH_THEME_DELAY_IN_MS)
        }
      }
    })
  }

  getCurrentTheme(el: HTMLElement): Theme {
    const classList = el.classList
    return themes.find(theme => classList.contains(`${THEME_PREFIX}${theme}`)) || themes[0]
  }

  switchTheme(target: HTMLElement) {
    const currentTheme = this.getCurrentTheme(target)
    const currentIndex = themes.indexOf(currentTheme)
    const nextIndex = (currentIndex + 1) % themes.length
    const newThemeClass = `${THEME_PREFIX}${themes[nextIndex]}`

    target.classList.remove(`${THEME_PREFIX}${currentTheme}`)
    target.classList.add(newThemeClass)

    if(this.themeLabel) this.themeLabel.innerHTML = themes[nextIndex]

    if (target === this.html) {
      this.currentTheme = themes[nextIndex]
    }
  }

  switchThemeSection() {
    this.allThemedSections.forEach(section => this.switchTheme(section))
  }

  isInputElement(element: HTMLElement): boolean {
    return ['INPUT', 'TEXTAREA', 'SELECT'].includes(element.tagName)
  }
}

const setThemeInit = new SetTheme()

export default setThemeInit
