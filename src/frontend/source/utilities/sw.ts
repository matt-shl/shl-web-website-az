export const supportsServiceWorkers = 'serviceWorker' in navigator

export const initServiceWorker = () => {
  if (supportsServiceWorkers) {
    window.addEventListener('load', () => {
      navigator.serviceWorker
        .register('/sw.js')
        .then(reg => {
          console.log(`Serviceworker - Registration succeeded. Scope is ${reg.scope}`)
        })
        .catch(err => {
          console.error(`Serviceworker - Registration failed with error ${err}`)
        })
    })
  }
}

export const removeServiceWorker = () => {
  if (supportsServiceWorkers) {
    navigator.serviceWorker.getRegistrations().then(registrations => {
      for (const registration of registrations) {
        registration.unregister()
      }
    })
  }
}
