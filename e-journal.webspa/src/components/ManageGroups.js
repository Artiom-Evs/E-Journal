import { GroupsClient } from './api-clients/JournalApiClient';
import { States, BaseManage } from './BaseManage'

const properties = [
    {
        id: 'id',
        name: 'ID',
        type: 'hidden'
    },
    {
        id: 'name',
        name: 'Наименование',
        type: 'text',
        placeholder: 'Введите наименование учебной группы'
    }
];

export class ManageGroups extends BaseManage {
    _client = null;

    constructor(props) {
        super(props);
        super._id = 'groups';
        super._h1Text = 'Учебные группы';
        super._createButtonText = 'Добавить новую группу';
        super._createH2Text = 'Добавление новой группы';
        super._editH2Text = 'Редактирование группы';
        super._properties = properties;

        this._client = new GroupsClient();
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