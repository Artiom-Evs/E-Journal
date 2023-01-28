import { MarkValuesClient, TrainingTypesClient } from './api-clients/JournalApiClient';
import { States, BaseManage } from './BaseManage'

const properties = [
    {
        id: 'id',
        name: 'ID',
        type: 'hidden'
    },
    {
        id: 'name',
        name: 'Значение оценок',
        type: 'text',
        placeholder: 'Введите наименование значения оценки'
    }
];

export class ManageMarkValues extends BaseManage {
    _client = null;

    constructor(props) {
        super(props);
        super._id = 'markValues';
        super._h1Text = 'Значения оценок';
        super._createButtonText = 'Добавить новеое значение оценок';
        super._createH2Text = 'Добавление нового значения оценок';
        super._editH2Text = 'Редактирование значение оценок';
        super._properties = properties;

        this._client = new MarkValuesClient();
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