import { GroupsClient, MarksClient, MarkValuesClient, StudentsClient, SubjectsClient, TeachersClient, TrainingsClient, TrainingTypesClient } from './api-clients/JournalApiClient';
import { States, BaseManage } from './BaseManage'

const properties = [
    {
        id: 'id',
        name: 'ID',
        type: 'hidden'
    },
    {
        id: 'studentId',
        name: 'Учащийся',
        type: 'select',
        options: [],
        default: '0',
        viewer: (prop, value) => {
            let option = prop.options.find(o => o.key === value);
            return option ? option.value : value;
        }
    },
    {
        id: 'trainingId',
        name: 'Учебное занятие',
        type: 'select',
        options: [],
        default: '0',
        viewer: (prop, value) => {
            let option = prop.options.find(o => o.key === value);
            return option ? option.value : value;
        }
    },
    {
        id: 'valueId',
        name: 'Отметка',
        type: 'select',
        options: [],
        default: '0',
        viewer: (prop, value) => {
            let option = prop.options.find(o => o.key === value);
            return option ? option.value : value;
        }
    }
];

export class ManageMarks extends BaseManage {
    _client = null;
    _studentsClient = null;
    _trainingsClient = null;
    _markValuesClient = null;

    constructor(props) {
        super(props);
        super._id = 'marks';
        super._h1Text = 'Отметки';
        super._createButtonText = 'Добавить новую отметку';
        super._createH2Text = 'Добавление новой отметки';
        super._editH2Text = 'Редактирование отметки';
        super._properties = properties;

        this._client = new MarksClient();
        this._studentsClient = new StudentsClient();
        this._trainingsClient = new TrainingsClient();
        this._markValuesClient = new MarkValuesClient();
    }

    
    componentDidMount() {
        this.loadData();
    }

    async loadData() {
        this.setState({pageState:States.Loading});

        let data = await this._client.Get();
        
        let students = await this._studentsClient.Get();
        students = students.map(s => {
            return { key: s.id, value: s.name };
        });
        properties.find(o => o.id === 'studentId').options = students;

        let trainings = await this._trainingsClient.Get();
        trainings = trainings.map(t => {
            return { key: t.id, value: t.subject.name };
        });
        properties.find(o => o.id === 'trainingId').options = trainings;
        
        let values = await this._markValuesClient.Get();
        values = values.map(v => {
            return { key: v.id, value: v.name };
        });
        properties.find(o => o.id === 'valueId').options = values;
        
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