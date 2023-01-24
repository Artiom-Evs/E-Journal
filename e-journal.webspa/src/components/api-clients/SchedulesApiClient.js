
export class SchedulesApiClient {
    _baseUrl = '';

    constructor(baseUrl) {
        this._baseUrl = baseUrl;
    }

    async GetGroup(startDate, endDate, group) {
        return this.Get('groups', group, startDate, endDate);
    }

    async GetTeacher(startDate, endDate, name) {
        return this.Get('teachers', name, startDate, endDate);
    }

    async Get(category, name, startDate, endDate) {
        let url = this.buildUriDateRange(category, name, startDate, endDate);
    
        return fetch(url)
            .then(r => {
                console.log(`===> URL: ${r.url}.`);
                return r.json();
            })
            .catch(e => {
                console.error(`===> Error occored while loading data from schedules API:\n${e}`);
            });
    }

    buildUriDateRange(category, name, startDate, endDate) {
        let url = this._baseUrl;

        if (category) {
            url += `/${category}`;
        }
    
        if (name) {
            url += `/${name}`;
        }
    
        if (startDate) {
            url += `?startDate=${startDate}`;
        }
    
        if (endDate) {
            url += `&endDate=${endDate}`;
        }
    
        return url;
    }
}

const apiClient = new SchedulesApiClient('/schedules');
export default apiClient;
