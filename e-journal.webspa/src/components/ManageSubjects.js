import { SubjectsClient, TeachersClient } from './api-clients/JournalApiClient';
import { States, BaseManage } from './BaseManage'

const properties = [
    {
        id: 'id',
        name: 'ID',
        type: 'hidden'
    },
    {
        id: 'name',
        name: 'Дисциплина',
        type: 'text',
        placeholder: 'Введите наименование дисциплиныя'
    }
];

export class ManageSubjects extends BaseManage {
    _client = null;

    constructor(props) {
        super(props);
        super._id = 'subjects';
        super._h1Text = 'Дисциплины';
        super._createButtonText = 'Добавить новую дисциплину';
        super._createH2Text = 'Добавление новой дисциплины';
        super._editH2Text = 'Редактирование дисциплины';
        super._properties = properties;

        this._client = new SubjectsClient();
    }

    
    componentDidMount() {
        this.loadData();
    }

    async loadData() {
        this.setState({pageState:States.Loading});

        let data = await this._client.Get();

        this.setState({
            items: data,
            pageState:States.Table
        });
    }

    async handleCreate(teacher) {
        let response = await this._client.Post(teacher);
        console.log(`Status: ${response.statusText}`);
        this.loadData();
    }

    async handleUpdate(teacher) {
        let response = await this._client.Put(teacher);
        console.log(`Status: ${response.statusText}`);
        this.loadData();
    }

    async handleDelete(teacher) {
        let response = await this._client.Delete(teacher.id);
        console.log(`Status: ${response.statusText}`);
        this.loadData();
    }
}