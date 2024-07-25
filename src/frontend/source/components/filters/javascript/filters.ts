import { html } from '@utilities/dom-elements'
import GTM from '@utilities/gtm'
import RafThrottle from '@utilities/raf-throttle'
import ScreenDimensions from '@utilities/screen-dimensions'
import { AxiosResponse } from 'axios'

import API from '@/utilities/api'
import Events from '@/utilities/events'
import { modifyHTML } from '@/utilities/modify-html'

const JS_HOOK_FILTERS = '[js-hook-filters]'
const JS_HOOK_FILTERS_MODAL = '[js-hook-filters-modal]'
const JS_HOOK_FILTERS_INPUT = '[js-hook-filters-input]'
const JS_HOOK_FILTERS_STICKY_BUTTON = '[js-hook-filters-sticky-button]'
const JS_HOOK_FILTERS_RESET_BUTTON = '[js-hook-filters-reset-button]'
const JS_HOOK_FILTERS_REFRESH_CONTAINER = '[js-hook-filters-refresh-container]'
const JS_HOOK_ACCORDION_DETAIL = '[js-hook-accordion-detail]'
const JS_HOOK_FILTER_TITLES = '[js-hook-accordion-item-title]'
const JS_HOOK_PLP_TITLE = '[js-hook-plp-header-title]'
const JS_HOOK_FILTERS_MODAL_CLOSE_BTN = '[js-hook-modal-close-btn]'

const DATA_ATTRIBUTE_PLP_TITLE = 'data-plp-filter-title'
const DATA_ATTRIBUTE_SORT_ITEM = 'data-sort-item'

const CLASS_IS_FILTERS_LIST_STICKY = 'is--filters-sticky'

class Filters {
  element: HTMLElement
  inputs: NodeListOf<HTMLInputElement> | undefined
  refreshContainer: HTMLElement | null
  stickyButtons: NodeListOf<HTMLButtonElement> | undefined
  resetButton: HTMLButtonElement | null
  allFilterOptionNames: HTMLElement[] | null
  hasItemsSelected: boolean
  plpTitleElement: HTMLElement | null
  ajaxEndpoint: any
  url: any
  category?: string

  constructor(element: HTMLElement) {
    this.element = element
    this.inputs = element.querySelectorAll(JS_HOOK_FILTERS_INPUT)
    this.refreshContainer = document.querySelector(JS_HOOK_FILTERS_REFRESH_CONTAINER)
    this.stickyButtons = element.querySelectorAll(JS_HOOK_FILTERS_STICKY_BUTTON)
    this.resetButton = element.querySelector(JS_HOOK_FILTERS_RESET_BUTTON)
    this.allFilterOptionNames = Array.from(element.querySelectorAll(JS_HOOK_FILTER_TITLES))
    this.plpTitleElement = document.querySelector(JS_HOOK_PLP_TITLE)
    this.hasItemsSelected = this.#hasItemsSeleted()
    this.ajaxEndpoint = ''
    this.category = document.querySelector('.product-listing-header__title')?.textContent?.trim()
    this.#bindEvents()
  }

  #bindEvents() {
    this.inputs?.forEach(element =>
      element.addEventListener('click', () => {
        this.#getNewResults(element)
      }),
    )
    this.stickyButtons?.forEach(element =>
      element.addEventListener('click', () => this.#showResults()),
    )
    this.resetButton?.addEventListener('click', () => {
      if (this.resetButton) {
        this.#getNewResults(this.resetButton)
      }
    })

    RafThrottle.set([
      {
        element: window,
        event: 'scroll',
        namespace: 'filters-scroll',
        fn: () => this.#toggleStickyClass(),
      },
    ])
  }

  #toggleStickyClass() {
    if (!this.element) return
    const { y, height } = this.element.getBoundingClientRect()
    console.log(ScreenDimensions.isTabletLandscapeAndBigger, y < height, y, height)

    // @TODO check with anne how the navigation is going to work
    if (
      (y === 0 && ScreenDimensions.isTabletLandscapeAndBigger) ||
      (y < height * 2 && html.classList.contains('header--is-going-up'))
    ) {
      html.classList.add(CLASS_IS_FILTERS_LIST_STICKY)
    } else {
      html.classList.remove(CLASS_IS_FILTERS_LIST_STICKY)
    }
  }
  handleAnchorClick(e: MouseEvent, anchor: HTMLAnchorElement) {
    e.preventDefault()
    const id = anchor.getAttribute('href') || ''
    const section = document.querySelector(id)
    if (!section) return
    Events.$trigger('scroll-to::scroll', {
      data: {
        target: section,
        offset: 100,
      },
    })
  }

  #hasItemsSeleted(): boolean {
    const checkedOptions = this.element.querySelectorAll('input:checked')
    return !!checkedOptions.length
  }

  #getNewResults(element?: HTMLInputElement | HTMLButtonElement, shouldPushState?: boolean) {
    this.url = element?.dataset.url
    this.ajaxEndpoint = element?.dataset.ajaxEndpoint || this.url

    if (this.ajaxEndpoint) {
      Events.$trigger('loader::show')
      this.#disableFilters()

      API.get(this.ajaxEndpoint)
        .then(response => this.newResultsSuccess(response, element, shouldPushState))
        .catch(error => this.#newResultsFail(error.response))
    }
  }

  #disableFilters() {
    // disable accordion items and fieldsets
    const accordionItems = this.element.querySelectorAll(JS_HOOK_ACCORDION_DETAIL)

    accordionItems.forEach(accordionItem => {
      const fieldset = accordionItem.querySelector('fieldset')
      accordionItem.setAttribute('disabled', '')
      fieldset?.setAttribute('disabled', '')
    })

    // disable sticky buttons
    this.stickyButtons?.forEach(button => button.setAttribute('disabled', ''))

    // disable close modal btns
    const filtersModalCloseBtns = this.element.querySelectorAll<HTMLButtonElement>(
      `${JS_HOOK_FILTERS_MODAL} ${JS_HOOK_FILTERS_MODAL_CLOSE_BTN}`,
    )

    filtersModalCloseBtns.forEach(button => {
      button.setAttribute('disabled', '')
    })
  }

  #showResults() {
    Events.$trigger('scroll-to::scroll', {
      data: {
        target: this.refreshContainer,
        offset: 40,
      },
    })
  }

  newResultsSuccess(
    response: AxiosResponse,
    element: HTMLInputElement | HTMLButtonElement | null | undefined,
    shouldPushState = true,
  ) {
    // remove previous modal from store
    const existingModal = <HTMLElement>this.element.querySelector(JS_HOOK_FILTERS_MODAL)
    Events.$trigger(`modal[${existingModal.id}]::remove`)

    const doc = new DOMParser().parseFromString(response.data, 'text/html')
    const newHtml = doc.querySelector(JS_HOOK_FILTERS_REFRESH_CONTAINER)
    const newTitle = doc
      .querySelector(`[${DATA_ATTRIBUTE_PLP_TITLE}]`)
      ?.getAttribute(DATA_ATTRIBUTE_PLP_TITLE)
    const openParentElementId = element?.closest(JS_HOOK_ACCORDION_DETAIL)?.id.toString()
    const modal = <HTMLElement>doc?.querySelector(JS_HOOK_FILTERS_MODAL)
    const filters = <HTMLElement>doc?.querySelector(JS_HOOK_FILTERS)

    if (openParentElementId) {
      doc.getElementById(openParentElementId)?.setAttribute('open', '')
    }

    if (newTitle && this.plpTitleElement) {
      this.plpTitleElement.textContent = newTitle
    }

    if (newHtml && this.refreshContainer) {
      modal.setAttribute('animated', '') // Permitted to prevent animation

      modifyHTML(this.refreshContainer, newHtml)
        .then(() => {
          // open filter modal
          Events.$trigger(`modal[${modal.id}]::open`)
          Events.$trigger('gtm::rebind', { data: newHtml })
          if (element?.dataset.ajaxEndpoint && shouldPushState && filters)
            this.#updateUrlAndHistoryState()
        })
        .catch(error => console.error('An error occurred: ', error))
    }
  }

  /* To-do: Add error states */
  #newResultsFail(_error: AxiosResponse) {
    Events.$trigger('loader::hide')
  }

  #updateUrlAndHistoryState() {
    const pushOptions = {
      state: {
        ajaxEndpoint: this.ajaxEndpoint,
        refinements: true,
      },
      url: this.url,
    }

    window.history.pushState(pushOptions.state, '', pushOptions.url)
  }
}

export default Filters
