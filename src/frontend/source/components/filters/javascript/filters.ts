import { html } from '@utilities/dom-elements'
import RafThrottle from '@utilities/raf-throttle'
import ReplaceContent from '@utilities/replace-content'
import ScreenDimensions from '@utilities/screen-dimensions'
import { AxiosResponse } from 'axios'

import API from '@/utilities/api'
import Events from '@/utilities/events'

const JS_HOOK_ACCORDION_DETAIL = '[js-hook-accordion-detail]'
const JS_HOOK_FILTERS_INPUT = '[js-hook-filters-input]'
const JS_HOOK_FILTERS_STICKY_BUTTON = '[js-hook-filters-sticky-button]'
const JS_HOOK_FILTERS_RESET_BUTTON = '[js-hook-filters-reset-button]'
const JS_HOOK_FILTER_TITLES = '[js-hook-accordion-item-title]'
const JS_HOOK_FILTERS_MODAL_CLOSE_BTN = '[js-hook-filters-button-modal-close]'
const JS_HOOK_FILTERS_SHOW_MORE_OPTIONS = '[js-hook-filters-show-more-options]'
const JS_HOOK_FORM = '[js-hook-form]'

const KEYBOARD_FOCUSED_CLASS = 'has--keyboard-focus'
const CLASS_MODAL_SCROLLING = 'is--modal-scrolling'
const CLASS_IS_FILTERS_LIST_STICKY = 'is--filters-sticky'
const CLASS_FILTERS_ACCORDION_OPTIONS = 'filters__accordion-options'
const CLASS_FILTERS_ACCORDION_OPTIONS_HIDDEN = 'filters__accordion-options--hidden'

class Filters {
  element: HTMLElement
  inputs: NodeListOf<HTMLInputElement> | undefined
  stickContainer: Element | null
  stickyButtons: NodeListOf<HTMLButtonElement> | undefined
  resetButton: HTMLButtonElement | null
  closeModelButton: HTMLButtonElement | null
  showMoreOptionsButtons: HTMLButtonElement[] | null
  allFilterOptionNames: HTMLElement[] | null
  hasItemsSelected: boolean
  endpoint: string | null
  urlReplacement: string | null
  filterFormElement: HTMLFormElement | null

  constructor(element: HTMLElement) {
    this.element = element
    this.stickContainer = this.element.children[0]
    this.#init()
  }

  // using init to bind events and to rebind events when the content is replaced
  #init() {
    this.inputs = this.element.querySelectorAll(JS_HOOK_FILTERS_INPUT)
    this.stickyButtons = this.element.querySelectorAll(JS_HOOK_FILTERS_STICKY_BUTTON)
    this.resetButton = this.element.querySelector(JS_HOOK_FILTERS_RESET_BUTTON)
    this.closeModelButton = this.element.querySelector(JS_HOOK_FILTERS_MODAL_CLOSE_BTN)
    this.showMoreOptionsButtons = Array.from(
      this.element.querySelectorAll(JS_HOOK_FILTERS_SHOW_MORE_OPTIONS),
    )
    this.allFilterOptionNames = Array.from(this.element.querySelectorAll(JS_HOOK_FILTER_TITLES))
    this.hasItemsSelected = this.#hasItemsSelected()
    this.filterFormElement = this.element.querySelector(JS_HOOK_FORM)
    this.#bindEvents()
  }

  #bindEvents() {
    Events.$on(`replaceContent::modal-body-modal-filters`, () => {
      setTimeout(() => {
        Events.$trigger("lazyimage::update")
        Events.$trigger("gtm::update")
        this.#init()
      }, 100)
    })

    // get new results when a filter is clicked
    this.inputs?.forEach(element =>
      element.addEventListener('click', () => {
        this.#sendGtmData(element)
        this.#getNewResults(element)
      }),
    )

    this.closeModelButton?.addEventListener('click', () => {
      Events.$trigger("modal[modal-filters]::close", { data: { modalId: 'modal-filters' } })
    })

    // get new results for reset button
    this.resetButton?.addEventListener('click', () => {
        this.#getNewResults(undefined)
        Events.$trigger("modal[modal-filters]::close", { data: { modalId: 'modal-filters' } })
      }
    )

    // show more options
    this.showMoreOptionsButtons?.forEach(element =>
      element.addEventListener('click', () => this.#showMoreOptions(element)),
    )

    // toggle sticky class
    RafThrottle.set([
      {
        element: window,
        event: 'scroll',
        namespace: 'filters-scroll',
        fn: () => this.#toggleStickyClass(),
      },
    ])

    // check the if the form is scolling. In modal.ts it checkes for the full body, but for the filters we only make the form scrollable
    if (this.filterFormElement) {
      RafThrottle.set([
        {
          element: this.filterFormElement,
          event: 'scroll',
          namespace: `Modal[filters]FormIsScrolling`,
          fn: () => {
            const action = this.filterFormElement!.scrollTop > 0 ? 'add' : 'remove'
            this.element.classList[action](CLASS_MODAL_SCROLLING)
          },
        },
      ])
    }
  }

  #hasItemsSelected(): boolean {
    const checkedOptions = this.element.querySelectorAll('input:checked')
    return !!checkedOptions.length
  }

  #getNewResults(element?: HTMLInputElement | HTMLButtonElement, shouldPushState?: boolean) {
    this.urlReplacement = this.#getNewResultsUrl(element)

    this.endpoint = this.urlReplacement

    if (this.endpoint) {
      Events.$trigger('loader::show')

      API.get(this.endpoint)
        .then(response => this.#newResultsSuccess(response, element, shouldPushState))
        .catch(error => {
          console.error('An error occurred: ', error)
          this.#newResultsFail(error.response)
        })
    }
  }

  #getNewResultsUrl(element?: HTMLInputElement | HTMLButtonElement) {
    const currentUrl = new URL(this.filterFormElement.action);

    if (!element) {
      return currentUrl.toString()
    }

    const queryParams = new URLSearchParams();

    this.inputs.forEach(input => {
      if (input) {
        if (input.type === 'checkbox' || input.type === 'radio') {
          if (input.checked) {
            queryParams.append(input.name, input.value);
          }
        } else if (input.value) {
          queryParams.append(input.name, input.value);
        }
      }
    });



    currentUrl.search = queryParams.toString();

    return currentUrl.toString()
  }

  #newResultsSuccess(
    response: AxiosResponse,
    element: HTMLInputElement | HTMLButtonElement | null | undefined,
    shouldPushState = true,
  ) {
    const doc = new DOMParser().parseFromString(response.data, 'text/html')
    const newHtml = doc.documentElement
    const openAccordionIds = [...this.element.querySelectorAll(`${JS_HOOK_ACCORDION_DETAIL}[open]`)].map(
      (el) => el.id,
    ) as string[]
    const expandedAccordionsIds = [...this.element.querySelectorAll(JS_HOOK_ACCORDION_DETAIL)].map(accordion => {
      if(!accordion.querySelector('.filters__accordion-options--hidden')) {
        return accordion.id
      } else {
        return false
      }
    })
    const selectedElementId = element?.id
    const hasKeyboardFocus = document.body.classList.contains(KEYBOARD_FOCUSED_CLASS)

    if (!newHtml) throw new Error('No new content found')

    if (openAccordionIds) {
      openAccordionIds.forEach((id) => {
        doc.getElementById(id)?.setAttribute('open', '')
      })
    }

    ReplaceContent.replaceAllContent(newHtml)

    if (selectedElementId) this.element.querySelector<HTMLInputElement | HTMLButtonElement>(`#${selectedElementId}`)?.focus()

    if (hasKeyboardFocus) document.body.classList.add(KEYBOARD_FOCUSED_CLASS)

    expandedAccordionsIds.forEach((expandedAccordionsId) => {
      if(expandedAccordionsId) {
        document.getElementById(expandedAccordionsId)?.querySelector(`.${CLASS_FILTERS_ACCORDION_OPTIONS}`)?.classList.remove(CLASS_FILTERS_ACCORDION_OPTIONS_HIDDEN)
      }
    })

    if (this.urlReplacement && shouldPushState) this.#updateUrlAndHistoryState()
  }

  #newResultsFail(_error: AxiosResponse) {
    console.error('An error occurred: ', _error)
    Events.$trigger('loader::hide')
  }

  #updateUrlAndHistoryState() {
    const pushOptions = {
      state: {
        endpoint: this.endpoint,
        refinements: true,
      },
      url: this.urlReplacement,
    }
    window.history.pushState(pushOptions.state, '', pushOptions.url)
  }

  #toggleStickyClass() {
    if (!this.element) return
    const origin = this.element.getBoundingClientRect()

    const scrollCondition = origin.top < 0

    html.classList[
      scrollCondition && ScreenDimensions.isTabletLandscapeAndBigger ? 'add' : 'remove'
      ](CLASS_IS_FILTERS_LIST_STICKY)
  }

  #showMoreOptions(element: HTMLButtonElement) {
    const optionsEl = element.previousElementSibling
    optionsEl?.classList.remove(CLASS_FILTERS_ACCORDION_OPTIONS_HIDDEN)
    const options =
      optionsEl && ([...optionsEl?.querySelectorAll(JS_HOOK_FILTERS_INPUT)] as HTMLInputElement[])
    if (options && options[4]) {
      options[4].focus()
    }
  }

  #sendGtmData(element: HTMLInputElement) {
    // Only send event when the element is getting selected
    if(element.getAttribute('checked') !== null) return

    const id = element.id
    const optionClicked = this.element.querySelector(`label[for="${id}"]`)?.textContent?.trim()
    Events.$trigger('gtm::push', {
      data: {
        'event': element.name.toLowerCase() === 'sort' ? 'sorting' : 'filtering',
        'option_clicked': optionClicked
      }
    })
  }
}

export default Filters
