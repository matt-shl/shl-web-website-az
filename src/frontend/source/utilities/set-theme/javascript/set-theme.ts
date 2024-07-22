import Environment from '@utilities/environment'

const themes = [
  'general',
  'pale-pink',
  'pale-green',
  'pale-yellow',
  'pale-blue',
  'pastel-blue',
  'light-blue',
  'white',
  'dark-pink',
  'light-grey',
] as const

type Theme = (typeof themes)[number]

const THEME_PREFIX = 't-'
const SWITCH_THEME_DELAY_IN_MS = 300

class SetTheme {
  html: HTMLElement
  currentTheme: Theme
  isSwitching: boolean

  constructor() {
    if (!Environment.isLocal && !Environment.isTest) return

    this.html = document.documentElement
    this.currentTheme = this.#getCurrentTheme()

    this.#bindEvents()
  }

  // When shift and T are pressed, switch theme
  #bindEvents() {
    document.addEventListener('keydown', event => {
      if (
        event.shiftKey &&
        event.key === 'T' &&
        !this.#isInputElement(event.target as HTMLElement)
      ) {
        this.#switchTheme()
      }
    })
  }

  // Get current theme on page load
  #getCurrentTheme(): Theme {
    const classList = this.html.classList

    for (const theme of themes) {
      if (classList.contains(`${THEME_PREFIX}${theme}`)) {
        return theme
      }
    }
    return themes[0]
  }

  // Switch theme
  #switchTheme() {
    if (this.isSwitching) return

    const currentIndex = themes.indexOf(this.currentTheme)
    const nextIndex = (currentIndex + 1) % themes.length
    const oldThemeClass = `${THEME_PREFIX}${this.currentTheme}`
    const newThemeClass = `${THEME_PREFIX}${themes[nextIndex]}`

    this.html.classList.remove(oldThemeClass)
    this.html.classList.add(newThemeClass)
    this.currentTheme = themes[nextIndex]

    this.isSwitching = true

    setTimeout(() => {
      this.isSwitching = false
    }, SWITCH_THEME_DELAY_IN_MS)
  }

  #isInputElement(element: HTMLElement): boolean {
    return ['INPUT', 'TEXTAREA', 'SELECT'].includes(element.tagName)
  }
}

const setThemeInit = new SetTheme()

export default setThemeInit
