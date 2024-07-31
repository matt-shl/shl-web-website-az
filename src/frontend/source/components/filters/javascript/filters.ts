import { html } from '@utilities/dom-elements'
import RafThrottle from '@utilities/raf-throttle'
import ReplaceContent from '@utilities/replace-content'
import ScreenDimensions from '@utilities/screen-dimensions'
import { AxiosResponse } from 'axios'

import API from '@/utilities/api'
import Events from '@/utilities/events'

const JS_HOOK_ACCORDION_DETAIL = '[js-hook-accordion-detail]'
const JS_HOOK_FILTERS_MODAL = '[js-hook-filters-modal]'
const JS_HOOK_FILTERS_INPUT = '[js-hook-filters-input]'
const JS_HOOK_FILTERS_STICKY_BUTTON = '[js-hook-filters-sticky-button]'
const JS_HOOK_FILTERS_RESET_BUTTON = '[js-hook-filters-reset-button]'
const JS_HOOK_FILTER_TITLES = '[js-hook-accordion-item-title]'
const JS_HOOK_FILTERS_MODAL_CLOSE_BTN = '[js-hook-button-modal-close]'
const JS_HOOK_FILTERS_SHOW_MORE_OPTIONS = '[js-hook-filters-show-more-options]'
const JS_HOOK_FORM = '[js-hook-form]'

const CLASS_MODAL_SCROLLING = 'is--modal-scrolling'
const CLASS_IS_FILTERS_LIST_STICKY = 'is--filters-sticky'
const CLASS_FILTERS_ACCORDION_OPTIONS_HIDDEN = 'filters__accordion-options--hidden'

class Filters {
  element: HTMLElement
  inputs: NodeListOf<HTMLInputElement> | undefined
  stickyButtons: NodeListOf<HTMLButtonElement> | undefined
  resetButton: HTMLButtonElement | null
  showMoreOptionsButtons: HTMLButtonElement[] | null
  allFilterOptionNames: HTMLElement[] | null
  hasItemsSelected: boolean
  endpoint: string | null
  urlReplacement: string | null
  filterFormElement: HTMLFormElement | null

  constructor(element: HTMLElement) {
    this.element = element
    this.#init()
  }

  // using init to bind events and to rebind events when the content is replaced
  #init() {
    this.inputs = this.element.querySelectorAll(JS_HOOK_FILTERS_INPUT)
    this.stickyButtons = this.element.querySelectorAll(JS_HOOK_FILTERS_STICKY_BUTTON)
    this.resetButton = this.element.querySelector(JS_HOOK_FILTERS_RESET_BUTTON)
    this.showMoreOptionsButtons = Array.from(
      this.element.querySelectorAll(JS_HOOK_FILTERS_SHOW_MORE_OPTIONS),
    )
    this.allFilterOptionNames = Array.from(this.element.querySelectorAll(JS_HOOK_FILTER_TITLES))
    this.hasItemsSelected = this.#hasItemsSeleted()
    this.filterFormElement = this.element.querySelector(JS_HOOK_FORM)
    this.#bindEvents()
  }

  #bindEvents() {
    Events.$on(`replaceContent::modal-body-modal-filters`, () => this.#init())

    // get new results when a filter is clicked
    this.inputs?.forEach(element =>
      element.addEventListener('click', () => {
        this.#getNewResults(element)
      }),
    )

    // get new results for reset button
    this.resetButton?.addEventListener('click', this.#getNewResults.bind(this, this.resetButton))

    // show more options
    this.showMoreOptionsButtons?.forEach(element =>
      element.addEventListener('click', this.#showMoreOptions.bind(this, element)),
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

  #hasItemsSeleted(): boolean {
    const checkedOptions = this.element.querySelectorAll('input:checked')
    return !!checkedOptions.length
  }

  #getNewResults(element?: HTMLInputElement | HTMLButtonElement, shouldPushState?: boolean) {
    this.urlReplacement = element?.dataset.urlReplacement || ''
    this.endpoint = element?.dataset.endpoint || this.urlReplacement

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

  #newResultsSuccess(
    response: AxiosResponse,
    element: HTMLInputElement | HTMLButtonElement | null | undefined,
    shouldPushState = true,
  ) {
    const doc = new DOMParser().parseFromString(response.data, 'text/html')
    const newHtml = doc.documentElement
    // get the new element to focus on, this needs to happen before the content is replaced
    const newSelectedElement = newHtml.querySelector(`#${element?.id}`)

    if (!newHtml) throw new Error('No new content found')

    // replace the content
    ReplaceContent.replaceAllContent(newHtml)

    // opens the accordion if the element is inside one + focus on the new element
    if (newSelectedElement) {
      const parentCarousel = newSelectedElement.closest('details')
      if (parentCarousel) {
        parentCarousel.open = true
      }
      ;(newSelectedElement as HTMLInputElement).focus()
    }

    if (element?.dataset.endpoint && shouldPushState) {
      this.#updateUrlAndHistoryState()
    }
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
    const { y, height } = this.element.getBoundingClientRect()

    html.classList[y < height && ScreenDimensions.isTabletLandscapeAndBigger ? 'add' : 'remove'](
      CLASS_IS_FILTERS_LIST_STICKY,
    )
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
}

export default Filters
