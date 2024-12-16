import Events from '@/utilities/events'
import Environment from "@utilities/environment";

export type GTMEntry = Record<string, any>

const DATA_REGISTERED = 'data-gtm-registered'

const HOOK_KNOWLEDGE_OVERVIEW_SECTIONS = '[js-hook-gtm-knowledge-overview-sections]'
const BUTTON_SELECTOR = '*[class*="c-button"]:not(.button--icon-only)'
const SECTION_SELECTOR = '[js-hook-section],*[class*="c-hero"],.c-event-detail'
const HERO_SELECTOR = '*[class*="c-hero"]'
const TITLE_ELEMENTS = 'h1,h2,h3,h4,h5,h6'
const APPLY_SELECTOR = '[js-hook-apply]'
const JS_HOOK_SENIORITY = '[js-hook-job-listing-seniority]'
const JS_HOOK_EMPLOYMENT = '[js-hook-job-listing-employment]'

const JS_HOOK_OVERVIEW = '[js-hook-gtm-overview]'
const CARD_SELECTOR = '*[class^="c-card"]:not(.c-card-overlay)'

class GTM {
  private knowledgeOverviewSections: HTMLElement[]
  private buttons: HTMLElement[]
  private cards: HTMLElement[]
  private overviewType: string
  private applyButtons: HTMLElement[]
  private isDebug: boolean

  constructor() {
    this.isDebug = window.location.search.includes('debug')

    this.init();
    this.bindDynamicEvents()
    this.bindEvents()
  }

  init() {
    this.applyButtons = [...document.querySelectorAll(APPLY_SELECTOR)] as HTMLElement[]
    this.knowledgeOverviewSections = [...document.querySelectorAll<HTMLElement>(`${HOOK_KNOWLEDGE_OVERVIEW_SECTIONS} ${CARD_SELECTOR}`)]
    this.buttons = [...document.querySelectorAll(BUTTON_SELECTOR)] as HTMLElement[]
    this.overviewType = document.querySelector(JS_HOOK_OVERVIEW)?.getAttribute('js-hook-gtm-overview') || ''
    this.cards = [...document.querySelectorAll(`${JS_HOOK_OVERVIEW} ${CARD_SELECTOR}`)] as HTMLElement[]
  }

  bindEvents() {
    Events.$on("gtm::update", () => {
      this.init()
      this.bindDynamicEvents()
    })

    Events.$on<GTMEntry>('gtm::push', (_, data) => this.push(data))
  }

  bindDynamicEvents() {
    this.applyButtons.forEach(button => {
      button.addEventListener('click', () => {
        const hero = document.querySelector(HERO_SELECTOR)
        if(!hero) return

        const jobName = hero.querySelector('h1')?.textContent?.trim()
        const location = hero.querySelector('.hero-content__subtitle')?.textContent?.trim()
        const seniorityLevel = hero.querySelector(JS_HOOK_SENIORITY)?.textContent?.trim()
        const employmentType = hero.querySelector(JS_HOOK_EMPLOYMENT)?.textContent?.trim()

        Events.$trigger('gtm::push', {
          data: {
            'event': 'apply_now_button',
            'job_name': jobName || "",
            'location': location || "",
            'seniority_level': seniorityLevel || "",
            'employment_type': employmentType || ""
          }
        })
      })
    });

    this.buttons.forEach(button => {
      if (button.getAttribute(DATA_REGISTERED) === 'true') return
      button.setAttribute(DATA_REGISTERED, 'true')
      this.initButtonEvent(button)
    })

    this.cards.forEach(card => {
      if (card.getAttribute(DATA_REGISTERED) === 'true') return
      card.setAttribute(DATA_REGISTERED, 'true')
      this.initCardEvent(card)
    })

    this.knowledgeOverviewSections.forEach(section => {
      section.addEventListener('click', () => {
        const optionClicked = section.querySelector('h3')?.textContent?.trim()

        Events.$trigger('gtm::push', {
          data: {
            'event': 'section_selected',
            'option_clicked': optionClicked
          }
        })
      })
    })
  }

  initCardEvent(card: HTMLElement) {
    card.addEventListener('click', () => {
      const optionClicked = card.querySelector('h3')?.textContent?.trim()

      if (!optionClicked) return

      Events.$trigger('gtm::push', {
        data: {
          'event': `${this.overviewType}_selected`,
          'option_clicked': optionClicked
        }
      })
    })
  }

  initButtonEvent(button: HTMLElement) {
    button.addEventListener('click', () => {
      const ctaName = button.textContent?.trim()
      const ctaPosition = button.closest(SECTION_SELECTOR)?.querySelector(TITLE_ELEMENTS)?.textContent?.trim() || document.querySelector(HERO_SELECTOR)?.querySelector(TITLE_ELEMENTS)?.textContent?.trim()

      if (!ctaPosition || !ctaName) return

      Events.$trigger('gtm::push', {
        data: {
          'event': 'cta_click',
          'cta_name': ctaName,
          'cta_position': ctaPosition
        }
      })
    })
  }

  push(data: GTMEntry) {
    let {dataLayer} = window

    if(!Environment.isProduction && this.isDebug) console.log("Debug mode, send to GTM:", data)

    dataLayer = dataLayer || []
    dataLayer.push(data)
  }
}

export default new GTM()
