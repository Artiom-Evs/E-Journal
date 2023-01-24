import { Component } from 'react'
import apiClient from './api-clients/SchedulesApiClient'
import { WeeklyTable } from './WeeklyTable';

export class WeeklySchedule extends Component {
    constructor(props) {
        super(props);

        let date = new Date();

        date.setDate(date.getDate() - date.getDay() + 1);
        let startDate = new Date(date);

        date.setDate(date.getDate() + 6);
        let endDate = new Date(date);

        this.state = {
            schedules: [],
            loading: true,
            startDate: startDate,
            endDate: endDate
        };

        console.log(`===> Start date: ${this.state.startDate}.`)
        console.log(`===> End date: ${this.state.endDate}.`)
    }

    componentDidMount() {
        this.populateSchedulesData();
    }

    render() {
        let { loading, startDate, endDate, schedules } = this.state;
        let { isGroup } = this.props;

        if (loading) {
            return (<div><em>Loading...</em></div>);
        }

        let h1 = isGroup ? 'Расписание групп на неделю' : 'Расписание преподавателей на неделю';
        let keys = Object.keys(schedules);
        
        return (
            <div>
                <h1>{h1}</h1>
                <div>
                    {keys.map((g, i) =>
                        <WeeklyTable key={g}
                            isGroup={isGroup}
                            title={g}
                            startDate={startDate}
                            endDate={endDate}
                            schedule={schedules[g]} />
                    )}
                </div>
            </div>
        );
    }

    async populateSchedulesData() {
        let { startDate, endDate } = this.state;
        let {isGroup} = this.props;

        let start = startDate.toISOString();
        let end = endDate.toISOString();

        start = start.slice(0, start.indexOf('T'));
        end = end.slice(0, end.indexOf('T'));

        let data = isGroup
            ? await apiClient.GetGroup(start, end)
            : await apiClient.GetTeacher(start, end);

        this.setState({ schedules: data, loading: false });
    }
}