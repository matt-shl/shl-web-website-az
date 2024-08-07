import Environment from '@utilities/environment'

const lang_classes = ['default', 'old', 'zh-hans', 'zh-hant', 'jp', 'kr'] as const

type Lang = (typeof lang_classes)[number]

const LANG_PREFIX = 'u-lang-'
const SWITCH_LANG_DELAY_IN_MS = 300

class SetLang {
  html: HTMLElement
  currentLang: Lang
  isSwitching: boolean

  constructor() {
    if (!Environment.isLocal && !Environment.isTest) return

    this.html = document.documentElement
    this.currentLang = this.getCurrentLang(this.html)

    this.bindEvents()
  }

  bindEvents() {
    document.addEventListener('keydown', event => {
      if (!this.isInputElement(event.target as HTMLElement)) {
        if (event.shiftKey && event.key === 'L') {
          if (this.isSwitching) return
          this.switchLang(this.html)
          this.isSwitching = true
          setTimeout(() => (this.isSwitching = false), SWITCH_LANG_DELAY_IN_MS)
        }
      }
    })
  }

  getCurrentLang(el: HTMLElement): Lang {
    const classList = el.classList
    return lang_classes.find(lang => classList.contains(`${LANG_PREFIX}${lang}`)) || lang_classes[0]
  }

  switchLang(target: HTMLElement) {
    const currentLang = this.getCurrentLang(target)
    const currentIndex = lang_classes.indexOf(currentLang)
    const nextIndex = (currentIndex + 1) % lang_classes.length
    const newLangClass = `${LANG_PREFIX}${lang_classes[nextIndex]}`

    target.classList.remove(`${LANG_PREFIX}${currentLang}`)
    target.classList.add(newLangClass)

    if (target === this.html) {
      this.currentLang = lang_classes[nextIndex]
    }
  }

  isInputElement(element: HTMLElement): boolean {
    return ['INPUT', 'TEXTAREA', 'SELECT'].includes(element.tagName)
  }
}

export default new SetLang()
