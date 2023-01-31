import { GroupsClient, SubjectsClient, TeachersClient, TrainingsClient, TrainingTypesClient } from './api-clients/JournalApiClient';
import { States, BaseManage } from './BaseManage'

const properties = [
    {
        id: 'id',
        name: 'ID',
        type: 'hidden'
    },
    {
        id: 'description',
        name: 'Описание',
        type: 'text',
        placeholder: 'Введите описание занятия'
    },
    {
        id: 'number',
        name: 'Номер пары',
        type: 'number',
        placeholder: 'Введите номер пары'
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
    },
    {
        id: 'teacherId',
        name: 'Преподаватель',
        type: 'select',
        options: [],
        default: '0',
        viewer: (prop, value) => {
            let option = prop.options.find(o => o.key === value);
            return option ? option.value : value;
        }
    },
    {
        id: 'subjectId',
        name: 'Учебный предмет',
        type: 'select',
        options: [],
        default: '0',
        viewer: (prop, value) => {
            let option = prop.options.find(o => o.key === value);
            return option ? option.value : value;
        }
    },
    {
        id: 'typeId',
        name: 'Тип занятия',
        type: 'select',
        options: [],
        default: '0',
        viewer: (prop, value) => {
            let option = prop.options.find(o => o.key === value);
            return option ? option.value : value;
        }
    }
];

export class ManageTrainings extends BaseManage {
    _client = null;
    _groupsClient = null;
    _subjectsClient = null;
    _teachersClient = null;
    _trainingTypesClient = null; 

    constructor(props) {
        super(props);
        super._id = 'trainings';
        super._h1Text = 'Учебные занятия';
        super._createButtonText = 'Добавить новое занятие';
        super._createH2Text = 'Добавление нового занятия';
        super._editH2Text = 'Редактирование занятия';
        super._properties = properties;

        this._client = new TrainingsClient();
        this._groupsClient = new GroupsClient();
        this._subjectsClient = new SubjectsClient();
        this._teachersClient = new TeachersClient();
        this._trainingTypesClient = new TrainingTypesClient();
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

        let subjects = await this._subjectsClient.Get();
        subjects = subjects.map(s => {
            return { key: s.id, value: s.name };
        });
        properties.find(o => o.id === 'subjectId').options = subjects;
        
        let teachers = await this._teachersClient.Get();
        teachers = teachers.map(t => {
            return { key: t.id, value: t.name };
        });
        properties.find(o => o.id === 'teacherId').options = teachers;
        
        let types = await this._trainingTypesClient.Get();
        types = types.map(t => {
            return { key: t.id, value: t.name };
        });
        properties.find(o => o.id === 'typeId').options = types;
        

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