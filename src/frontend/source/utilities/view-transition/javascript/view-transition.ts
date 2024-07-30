// @ts-nocheck

export const viewTransition = (fn: VoidFunction) => {
  return new Promise((resolve, reject) => {
    try {
      if (!document.startViewTransition) {
        fn()
        resolve()
      } else {
        document.startViewTransition(() => {
          fn()
          resolve()
        })
      }
    } catch (error) {
      reject(error)
    }
  })
}
