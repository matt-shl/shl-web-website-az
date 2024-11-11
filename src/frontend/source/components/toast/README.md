## Install

Import module

```javascript
moduleInit.async('[js-hook-toast]', () => import('@/components/toast'))
```

```htmlmixed
{% include 'toast.html' %}
```

## How to use

```javascript
Events.$trigger('toastManager::add', {
  data: { title: 'Title', body: 'Body', status: 'error' },
})
```
