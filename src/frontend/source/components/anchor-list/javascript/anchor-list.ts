import {html} from "@utilities/dom-elements";
import Events from "@utilities/events";
import RafThrottle from "@utilities/raf-throttle";
import ScreenDimensions from "@utilities/screen-dimensions";

const SVG_ARROW_RIGHT = '<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 17 16"><path fill="currentColor" d="m14.002 8.02-4.5 4.5a.5.5 0 0 1-.707-.707l3.647-3.646H2.648a.5.5 0 1 1 0-1h9.794L8.795 3.52a.5.5 0 1 1 .707-.708l4.5 4.5a.499.499 0 0 1 0 .708Z"></path></svg>'
const JS_HOOK_SECTION = '[js-hook-section]'
const JS_HOOK_HERO = '[js-hook-hero]'
const JS_HOOK_LIST = '[js-hook-anchor-list-list]'
const JS_HOOK_ANCHOR = '[js-hook-anchor-list-anchor]'
const CLASS_IS_COMPONENT = 'c-anchor-list--is-component'
const CLASS_HAS_ANCHOR_LINKS = 'has--anchor-links'
const CLASS_IS_ACTIVE = 'is--active'
const CLASS_IS_ANCHOR_LIST_STICKY = 'is--anchor-list-sticky'
const CLASS_IS_OUT_VIEW = "is--out-view"

class AnchorList {
  private element: HTMLElement;
  private list: HTMLElement | null;
  private anchors: HTMLAnchorElement[] | null;
  private sections: HTMLElement[];
  private hero: Element | null;

  constructor(element: HTMLElement) {
    this.element = element;
    this.hero = document.querySelector(JS_HOOK_HERO)
    this.list = this.element.querySelector(JS_HOOK_LIST)
    this.sections = Array.from(document.querySelectorAll(JS_HOOK_SECTION))

    this.createAnchorLinks();
    this.anchors = Array.from(this.element.querySelectorAll(JS_HOOK_ANCHOR))
    this.bindAnchorEvents();

    if(!this.element.classList.contains(CLASS_IS_COMPONENT)) return;

    this.initComponent();
    this.bindEvents()
  }

  initComponent() {
    html.classList.add(CLASS_HAS_ANCHOR_LINKS)

    this.toggleStickyClass();

    this.sections.forEach(section => {
      this.checkSectionVisibility(section);
    })
  }

  createAnchorLinks() {
    let html = '';

    this.sections.forEach((section) => {
      const title = section.getAttribute('data-title');
      const id = section.getAttribute('id');
      html += `<li class="anchor-list__item">
                  <a href="#${id}" class="anchor-list__anchor" js-hook-anchor-list-anchor>
                      ${SVG_ARROW_RIGHT}
                      ${title}
                  </a>
                </li>`
    })

    if (this.list) this.list.innerHTML = html;
  }

  bindAnchorEvents() {
    this.anchors?.forEach(anchor => {
      anchor.addEventListener("click", (e) => this.handleAnchorClick(e, anchor))
    })
  }

  bindEvents() {
    Events.$on("anchor-list::update", data => this.checkSectionVisibility(data.detail.data))

    RafThrottle.set([
      {
        element: window,
        event: 'scroll',
        namespace: 'anchor-list-scroll',
        fn: () => this.toggleStickyClass(),
      },
    ])
  }

  toggleStickyClass() {
    if(!this.hero) return;
    const {y, height} = this.hero.getBoundingClientRect()
    html.classList[(y < height * -1) && ScreenDimensions.isTabletLandscapeAndBigger ? 'add' : 'remove'](CLASS_IS_ANCHOR_LIST_STICKY)
  }

  handleAnchorClick(e: MouseEvent, anchor: HTMLAnchorElement) {
    e.preventDefault();
    const id = anchor.getAttribute('href') || '';
    const section = document.querySelector(id);
    if (!section) return;

    Events.$trigger('scroll-to::scroll', {
      data: {
        target: section,
        offset: 100,
      },
    })
  }

  checkSectionVisibility(section: HTMLElement) {
    if (section.classList.contains(CLASS_IS_OUT_VIEW)) return;

    this.setActiveAnchor(section)
  }

  setActiveAnchor(section: HTMLElement) {
    const id = section.getAttribute('id');
    const activeAnchor = this.element.querySelector(`[href="#${id}"]`);
    if (!activeAnchor || !this.anchors) return;

    this.anchors.forEach((anchor) => {
      anchor.classList.remove(CLASS_IS_ACTIVE)
    })

    activeAnchor.classList.add(CLASS_IS_ACTIVE)
  }
}

export default AnchorList
