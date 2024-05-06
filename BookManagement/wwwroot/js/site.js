const customFetch = (...params) =>
    fetch(...params)
        .then(rs => rs.json())
        .catch(err => {
            alert(err)
            console.log(err)
        })

let currentUser
(async function getCurrentUser() {
    const res = await customFetch('/Auth/GetCurrentUser')
    currentUser = res?.user
})()

const app = {
    page: {
        baseUrl: '/api/page',
        getBookWithPages(bookId) {
            const paramStr = new URLSearchParams({ bookId }).toString()
            return customFetch(this.baseUrl + '?' + paramStr)
        },
        getOne(id) {
            return customFetch(this.baseUrl + '/' + id)
        },
        create(body) {
            return customFetch(this.baseUrl, { method: 'POST', body })
        },
        update(id, body) {
            return customFetch(this.baseUrl + '/' + id, { method: 'PUT', body })
        },
        delete(id) {
            return fetch(this.baseUrl + '/' + id, { method: 'DELETE' })
        },
        getUserBookmarks() {
            return customFetch(this.baseUrl + '/bookmarks')
        },
        bookmark(id) {
            return customFetch(this.baseUrl + '/bookmark/' + id, { method: 'POST' })
        }
    }
}

const bookHubConnection = new signalR.HubConnectionBuilder()
    .withUrl('/bookhub')
    .build()

bookHubConnection.start()
bookHubConnection.onclose(err => console.log(err))