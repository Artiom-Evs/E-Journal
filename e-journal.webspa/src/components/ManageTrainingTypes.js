import { TrainingTypesClient } from './api-clients/JournalApiClient';
import { States, BaseManage } from './BaseManage'

const properties = [
    {
        id: 'id',
        name: 'ID',
        type: 'hidden'
    },
    {
        id: 'name',
        name: 'Тип занятий',
        type: 'text',
        placeholder: 'Введите наименование типа занятий'
    }
];

export class ManageTrainingTypes extends BaseManage {
    _client = null;

    constructor(props) {
        super(props);
        super._id = 'trainingTypes';
        super._h1Text = 'Типы занятий';
        super._createButtonText = 'Добавить новый тип занятий';
        super._createH2Text = 'Добавление нового типа занятий';
        super._editH2Text = 'Редактирование типа занятий';
        super._properties = properties;

        this._client = new TrainingTypesClient();
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