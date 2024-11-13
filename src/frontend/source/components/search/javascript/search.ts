import Events from '@utilities/events'

const JS_HOOK_FORM = '[js-hook-search-form]'
const JS_HOOK_INPUT = '[js-hook-search-input]'
const JS_HOOK_INPUT_RESET = '[js-hook-search-input-reset]'

const HIDDEN_CLASS = 'u-hidden'
const JOB_SEARCH_CLASS = 'c-search--job'

class Search {
  element: HTMLElement
  private form: HTMLFormElement | null
  private input: HTMLInputElement | null
  private inputReset: HTMLButtonElement | null
  private isJobSearch: boolean

  constructor(element: HTMLElement) {
    this.element = element
    this.form = this.element.querySelector(JS_HOOK_FORM)
    this.input = this.element.querySelector(JS_HOOK_INPUT)
    this.inputReset = this.element.querySelector(JS_HOOK_INPUT_RESET)
    this.isJobSearch = this.element.classList.contains(JOB_SEARCH_CLASS)

    this.#bindEvents()
  }

  #bindEvents() {
    this.input?.addEventListener('keydown', () => this.onTyping())

    this.inputReset?.addEventListener('click', event => this.resetInput(event))

    Events.$on('modal[navigation-mobile-search]::open', () => this.onOpenFlyout())

    this.form?.addEventListener('submit', () => {
      if (this.isJobSearch) {
        const location = this.form?.querySelector<HTMLSelectElement>('select[name="Country"]')?.value
        const seniority = this.form?.querySelector<HTMLSelectElement>('select[name="JobLevel"]')?.value
        const employment = this.form?.querySelector<HTMLSelectElement>('select[name="ContractType"]')?.value

        Events.$trigger('gtm::push', {
          data: {
            'event': 'search_jobs',
            'search_term': this.input?.value,
            'location': location || 'not selected',
            'seniority_level': seniority || 'not selected',
            'employment_type': employment || 'not selected',
          }
        })
      } else {
        Events.$trigger('gtm::push', {
          data: {
            'event': 'general_search',
            'search_term': this.input?.value
          }
        })
      }
    })
  }

  onTyping() {
    const valueLength = this.input?.value?.length
    this.inputReset?.classList[valueLength ? 'remove' : 'add'](HIDDEN_CLASS)
  }

  onOpenFlyout() {
    this.input?.focus()
  }

  resetInput(event: Event) {
    event.preventDefault()
    if (this.input) {
      this.input.value = ''
      this.input.focus()
    }

    this.inputReset?.classList.add(HIDDEN_CLASS)
  }
}

export default Search
