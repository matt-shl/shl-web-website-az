type DynamicImportType = () => Promise<{ default: any } | undefined | null | void>
interface ModuleInitHTMLElement extends HTMLElement {
  _initializedModules?: string[]
}

class ModuleInit {
  async(selector: string, moduleFn: DynamicImportType, opt_arguments?: any[]) {
    return new Promise(resolve => {
      const elements = this.findElements(selector)

      if (!elements.length) return resolve([])

      moduleFn().then(constructor => {
        const constructors = this.findElements(selector).map(element => {
          if (constructor) return this.loadConstructor(element, constructor.default, opt_arguments)
          return moduleFn
        })

        resolve(constructors)
      })
    })
  }

  sync(selector: string, constructor: any, opt_arguments?: any[]) {
    this.findElements(selector).forEach(element =>
      this.loadConstructor(element, constructor, opt_arguments),
    )
  }

  loadConstructor(element: ModuleInitHTMLElement, constructor: any, opt_arguments?: any[]) {
    element._initializedModules = element._initializedModules || []

    if (element._initializedModules.indexOf(constructor.name) !== -1) return

    element._initializedModules.push(constructor.name)

    if (opt_arguments) {
      const constructorArguments = [null, element]
      Array.prototype.push.apply(constructorArguments, opt_arguments)
      // eslint-disable-next-line prefer-spread
      return new (constructor.bind.apply(constructor, constructorArguments))()
    }

    if (typeof constructor === 'object') return constructor
    return new constructor(element)
  }

  findElements(selector: string) {
    return [...document.querySelectorAll<HTMLElement>(selector)]
  }
}

// IE polyfill for constructor.name
(function () {
  if (Function.prototype.name === undefined && Object.defineProperty !== undefined) {
    Object.defineProperty(Function.prototype, 'name', {
      get() {
        const funcNameRegex = /function\s([^(]{1,})\(/
        const results = funcNameRegex.exec(this.toString())
        return results && results.length > 1 ? results[1].trim() : ''
      },
      set() {
        // Empty function to prevent set is not a function
      },
    })
  }
})()

const moduleInit = new ModuleInit()

// Export the module init function
export default moduleInit
