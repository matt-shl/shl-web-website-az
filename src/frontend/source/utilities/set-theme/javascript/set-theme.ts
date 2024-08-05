import Environment from '@utilities/environment'

const themes = [
  'general',
  'lightest-pink',
  'pale-pink',
  'pale-green',
  'pale-yellow',
  'lightest-yellow',
  'pale-blue',
  'pastel-blue',
  'light-blue',
  'white',
  'dark-pink',
  'light-grey',
  'dark-green',
  'pastel-green'
] as const

type Theme = (typeof themes)[number]

const THEME_PREFIX = 't-'
const SWITCH_THEME_DELAY_IN_MS = 300

class SetTheme {
  html: HTMLElement
  currentTheme: Theme
  isSwitching: boolean
  allThemedSections: HTMLElement[]

  constructor() {
    if (!Environment.isLocal && !Environment.isTest) return

    this.html = document.documentElement
    this.allThemedSections = Array.from(document.querySelectorAll('.c-layout-section'))
    this.currentTheme = this.getCurrentTheme(this.html)

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
    console.log('switchTheme')
    const currentTheme = this.getCurrentTheme(target)
    const currentIndex = themes.indexOf(currentTheme)
    const nextIndex = (currentIndex + 1) % themes.length
    const newThemeClass = `${THEME_PREFIX}${themes[nextIndex]}`

    target.classList.remove(`${THEME_PREFIX}${currentTheme}`)
    target.classList.add(newThemeClass)

    if (target === this.html) {
      this.currentTheme = themes[nextIndex]
    }
  }

  switchThemeSection() {
    console.log(this.allThemedSections)
    this.allThemedSections.forEach(section => this.switchTheme(section))
  }

  isInputElement(element: HTMLElement): boolean {
    return ['INPUT', 'TEXTAREA', 'SELECT'].includes(element.tagName)
  }
}

const setThemeInit = new SetTheme()

export default setThemeInit
