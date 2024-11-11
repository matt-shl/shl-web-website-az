import Events from "@utilities/events";

const JS_HOOK_TITLE = '[js-hook-job-listing-title]'
const JS_HOOK_LOCATION = '[js-hook-job-listing-location]'
const JS_HOOK_SENIORITY = '[js-hook-job-listing-seniority]'
const JS_HOOK_EMPLOYMENT = '[js-hook-job-listing-employment]'

class JobListingItem {
  private element: HTMLElement

  constructor(element: HTMLElement) {
    this.element = element

    this.bindEvents()
  }

  bindEvents() {
    this.element.addEventListener('click', () => {
      const jobName = this.element.querySelector(JS_HOOK_TITLE)?.textContent?.trim()
      const location = this.element.querySelector(JS_HOOK_LOCATION)?.textContent?.trim()
      const seniorityLevel = this.element.querySelector(JS_HOOK_SENIORITY)?.textContent?.trim()
      const employmentType = this.element.querySelector(JS_HOOK_EMPLOYMENT)?.textContent?.trim()

      Events.$trigger('gtm::push', {
        data: {
          'event': 'select_job',
          'job_name': jobName || '',
          'location': location || '',
          'seniority_level': seniorityLevel || '',
          'employment_type': employmentType || ''
        }
      })
    })
  }
}

export default JobListingItem
