import { GroupsClient, StudentsClient} from './api-clients/JournalApiClient';
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
        placeholder: 'Введите инициалы учащегося'
    },
    {
        id: 'groupId',
        name: 'Учебная группа',
        type: 'select',
        options: [],
        default: '0',
        viewer: (prop, value) => {
            let option = prop.options.find(o => o.key === value);
            return option ? option.value : value;
        }
    }
];

export class ManageStudents extends BaseManage {
    _client = null;
    _groupsClient = null;

    constructor(props) {
        super(props);
        super._id = 'students';
        super._h1Text = 'Учащиеся';
        super._createButtonText = 'Добавить нового учащегося';
        super._createH2Text = 'Добавление нового учащегося';
        super._editH2Text = 'Редактирование учащегося';
        super._properties = properties;

        this._client = new StudentsClient();
        this._groupsClient = new GroupsClient();
    }

    
    componentDidMount() {
        this.loadData();
    }

    async loadData() {
        this.setState({pageState:States.Loading});

        let data = await this._client.Get();
        let groups = await this._groupsClient.Get();
        groups = groups.map(g => {
            return {key:g.id,value:g.name};
        });
        properties.find(o => o.id === 'groupId').options = groups;

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