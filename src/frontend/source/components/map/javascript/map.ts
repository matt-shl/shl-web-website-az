import Events from "@utilities/events"

const EUROPE = {
  id: "europe",
  paths: [
    "AL", "AD", "AM", "AT", "AZ", "BY", "BE", "BA", "BG", "HR", "CY", "CZ",
    "DK", "EE", "FI", "FR", "GE", "DE", "GR", "HU", "IE", "IT",
    "XK", "LV", "LI", "LT", "LU", "MT", "MD", "MC", "ME", "NL", "MK", "NO",
    "PL", "PT", "RO", "SM", "RS", "SK", "SI", "ES", "SE", "CH", "TR",
    "UA", "GB", "VA"
  ],
  zoom: 3,
  x: -2,
  y: 10
}

const AMERICAS = {
  id: "americas",
  paths: [
    "AG", "AI", "AR", "AW", "BS", "BB", "BZ", "BM", "BO", "BR", "VG", "CA", "KY",
    "CL", "CO", "CR", "CU", "CW", "DM", "DO", "EC", "SV", "FK", "GF", "GL", "GD",
    "GP", "GT", "GY", "HT", "HN", "JM", "MQ", "MX", "MS", "NI", "PA", "PY", "PE",
    "PR", "BL", "KN", "LC", "MF", "VC", "SX", "SR", "TT", "TC", "US", "UY", "VE"
  ],
  zoom: 1.5,
  x: 28,
  y: -4
}

const ASIA = {
  id: "asia",
  paths: [
    "AF", "AM", "AZ", "BH", "BD", "BT", "BN", "KH", "CN", "CY", "GE", "IN", "ID",
    "IR", "IQ", "IL", "JP", "JO", "KZ", "KW", "KG", "LA", "LB", "MY", "MV", "MN",
    "MM", "NP", "KP", "OM", "PK", "PS", "PH", "QA", "SA", "SG", "KR", "LK", "SY",
    "TJ", "TH", "TL", "TM", "AE", "UZ", "VN", "YE", "TW"
  ],
  zoom: 3,
  x: -21,
  y: -5
}

const JS_HOOK_MAP = "[js-hook-map-svg] svg"
const JS_HOOK_OFFICE_TRIGGER = '[js-hook-office-trigger]'
const JS_HOOK_CONTINENT_TRIGGER = '[js-hook-continent-trigger]'
const JS_HOOK_COUNTRY_TRIGGER = '[js-hook-country-trigger]'

const OFFICE_TRIGGER_IN_MAP_CLASS = 'map__office-trigger'

const ACTIVE_CLASS = 'is--active'
const INACTIVE_CLASS = 'is--inactive'
const HIDDEN_CLASS = 'is--hidden'

interface Continent {
  id: string
  paths: string[]
}

class Map {
  element: HTMLElement
  private svgMap: SVGElement
  private officeTriggers: HTMLButtonElement[]
  private continentTriggers: HTMLButtonElement[]
  private countryTriggers: HTMLButtonElement[]
  private allTriggersInMap: HTMLButtonElement[]
  private paths: SVGPathElement[]
  private currentLevel: string

  constructor(element: HTMLElement) {
    this.element = element
    this.svgMap = this.element.querySelector(JS_HOOK_MAP)!
    this.paths = [...this.svgMap.querySelectorAll("path")]
    this.officeTriggers = [...this.element.querySelectorAll<HTMLButtonElement>(JS_HOOK_OFFICE_TRIGGER)]
    this.continentTriggers = [...this.element.querySelectorAll<HTMLButtonElement>(JS_HOOK_CONTINENT_TRIGGER)]
    this.countryTriggers = [...this.element.querySelectorAll<HTMLButtonElement>(JS_HOOK_COUNTRY_TRIGGER)]
    this.allTriggersInMap = [...this.officeTriggers.filter(trigger => !trigger.classList.contains(OFFICE_TRIGGER_IN_MAP_CLASS)), ...this.continentTriggers, ...this.countryTriggers]

    this.init()
    this.bindEvents()
  }

  bindEvents() {
    this.svgMap.addEventListener('click', () => this.goBackLevel())

    this.continentTriggers.forEach(trigger => {
      trigger.addEventListener('click', () => this.handleContinentTriggerClick(trigger))
    })

    this.countryTriggers.forEach(trigger => {
      trigger.addEventListener('click', () => this.handleCountryTriggerClick(trigger))
    })

    this.officeTriggers.forEach(trigger => {
      trigger.addEventListener('click', () => {
        this.handleCountryTriggerClick(trigger)
        this.handleOfficeTriggerClick(trigger)
      })

      Events.$on(`modal[map-office-${trigger.dataset.officeId}]::close`, () => {
        this.resetOfficeTriggers()
      })
    })
  }

  init() {
    this.resetMap(true)
    this.setupContinent(EUROPE)
    this.setupContinent(AMERICAS)
    this.setupContinent(ASIA)
  }

  setupContinent({id, paths}: Continent) {
    paths.forEach((pathId) => {
      const path = this.svgMap.querySelector(`#${pathId}`)
      if (path) {
        path.classList.add(`map__path-${id}`)
      }
    })

    Events.$on(`accordion[${id}]::opening`, () => this.highlightContinent(id))
    Events.$on(`accordion[${id}]::closing`, () => this.resetMap(true))
  }

  resetMap(setLevel = false) {
    this.svgMap.setAttribute("class", "")
    this.svgMap.setAttribute("style", "")
    this.svgMap.setAttribute("data-level", "")

    this.paths.forEach(path => {
      path.classList.remove(ACTIVE_CLASS, INACTIVE_CLASS)
    })

    if (setLevel) {
      this.setLevel("world")
    }
  }

  highlightContinent(id: string) {
    const obj = id === 'europe' ? EUROPE : id === 'asia' ? ASIA : AMERICAS
    this.svgMap.classList.add(`is-${id}-highlighted`)
    this.svgMap.style.transform = `scale(${obj.zoom}) translateX(${obj.x}%) translateY(${obj.y}%)`
    this.setLevel("continent", id)
  }

  highlightCountry(id: string, zoom: string, x: string, y: string) {
    this.resetMap()
    this.svgMap.classList.add(`is-country-highlighted`)
    this.svgMap.style.transform = `scale(${zoom}) translateX(${x}%) translateY(${y}%)`
    this.setLevel("country", id)

    this.paths.forEach(path => {
      if (path.id == id) path.classList.add(ACTIVE_CLASS)
    })
  }

  setLevel(level: string, id?: string) {
    const value = level === 'world' ? 'world' : id
    if (!value) return

    this.currentLevel = value
    this.svgMap.setAttribute("data-level", value)

    this.allTriggersInMap.forEach(trigger => {
      const {zoomLevelVisible} = trigger.dataset
      const showTrigger = value === zoomLevelVisible
      trigger.classList[showTrigger ? 'remove' : 'add'](HIDDEN_CLASS)
      trigger.tabIndex = showTrigger ? 0 : -1
    })
  }

  handleContinentTriggerClick(trigger: Element) {
    const id = trigger.getAttribute('data-continent-id')
    if (!id) return

    Events.$trigger(`accordion[${id}]::open`)
  }

  handleCountryTriggerClick(trigger: HTMLButtonElement) {
    const {countryId, countryZoom, countryX, countryY} = trigger.dataset
    if (!countryId || !countryZoom || !countryX || !countryY) return

    this.highlightCountry(countryId, countryZoom, countryX, countryY)
  }

  handleOfficeTriggerClick(trigger: HTMLButtonElement) {
    const {officeId} = trigger.dataset
    if (!officeId) return

    Events.$trigger(`modal[map-office-${officeId}]::open`)
    this.setActiveOfficeTrigger(officeId)
  }

  setActiveOfficeTrigger(officeId: string) {
    this.officeTriggers.forEach(trigger => {
      trigger.classList.add(trigger.dataset.officeId === officeId ? ACTIVE_CLASS : INACTIVE_CLASS)
    })
  }

  resetOfficeTriggers() {
    this.officeTriggers.forEach(trigger => {
      trigger.classList.remove(ACTIVE_CLASS, INACTIVE_CLASS)
    })
  }

  goBackLevel() {
    switch (this.currentLevel) {
      case 'world':
        break

      case 'europe':
      case 'asia':
      case 'americas':
        Events.$trigger(`accordion[${this.currentLevel}]::close`)
        break

      default:
        Events.$trigger(`accordion[${this.getContinentIdFromCountryId(this.currentLevel)}]::open`)
        break
    }
  }

  getContinentIdFromCountryId(countryId: string) {
    if (EUROPE.paths.includes(countryId)) return 'europe'

    if (ASIA.paths.includes(countryId)) return 'asia'

    return 'americas'
  }
}

export default Map
