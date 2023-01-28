import { TeachersClient } from './api-clients/JournalApiClient';
import { States, BaseManage } from './BaseManage'

const properties = [
    {
        id: 'id',
        name: 'ID',
        type: 'hidden'
    },
    {
        id: 'name',
        name: 'Инициалы',
        type: 'text',
        placeholder: 'Введите инициалы преподавателя'
    }
];

export class ManageTeachers extends BaseManage {
    _client = null;

    constructor(props) {
        super(props);
        super._id = 'teachers';
        super._h1Text = 'Преподаватели';
        super._createButtonText = 'Добавить нового преподавателя';
        super._createH2Text = 'Добавление нового преподавателя';
        super._editH2Text = 'Редактирование преподавателя';
        super._properties = properties;

        this._client = new TeachersClient();
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