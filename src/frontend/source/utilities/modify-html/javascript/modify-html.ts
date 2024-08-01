import Events from '@utilities/events'

export const modifyHTML = (container, html, append = false) => {
  return new Promise<void>((resolve, reject) => {
    try {
      const modifyEvent = append ? appendEvent : replaceEvent

      if (!(document as Document & { startViewTransition?: any }).startViewTransition) {
        modifyEvent(container, html)
        resolve()
      } else {
        ;(document as Document & { startViewTransition?: any }).startViewTransition(() => {
          modifyEvent(container, html)
          resolve()
        })
      }
    } catch (error) {
      reject(error)
    }
  })
}

const replaceEvent = (container, html) => {
  container.replaceWith(html)

  Events.$trigger('modules::reinit')
  Events.$trigger('events::dom-reinit')
  Events.$trigger('modals::reInitAfterDOMInjection', { data: html })
}

const appendEvent = (container, html) => {
  while (container.firstChild) container.firstChild.remove()
  container.insertAdjacentHTML('beforeend', html)

  Events.$trigger('modules::reinit')
  Events.$trigger('events::dom-reinit')
  Events.$trigger('modals::reInitAfterDOMInjection', { data: container })
}
