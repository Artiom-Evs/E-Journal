
class BaseClient {
    _baseUrl = '';

    constructor(baseUrl) {
        this._baseUrl = baseUrl;
    }

    async Get() {
        return fetch(this._baseUrl)
            .then(r => {
                console.log(`===> URL: ${r.url}.`);
                return r.json();
            })
            .catch(e => {
                console.error(`===> Error occored while loading data from Journal API:\n${e}`);
            });
    }

    Post(model) {
        return fetch(this._baseUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify(model)
        });
    }

    Put(model) {
        return fetch(`${this._baseUrl}/${model.id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify(model)
        });
    }

    Delete(modelId) {
        return fetch(`${this._baseUrl}/${modelId}`, {
            method: 'DELETE'
        });
    }
}

export class GroupsClient extends BaseClient {
    constructor() {
        super('/journal/groups');
    }

    async Get(groupId) {
        let url = this._baseUrl;

        if (groupId) {
            url += `/${groupId}`;
        }

        return fetch(url)
            .then(r => {
                console.log(`===> URL: ${r.url}.`);
                return r.json();
            })
            .catch(e => {
                console.error(`===> Error occored while loading groups from Journal API:\n${e}`);
            });
    }
}