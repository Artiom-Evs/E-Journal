
const range = (start, stop) =>
    Array.from({ length: stop - start }, (y, i) => i + start);

const dateEqualityComparer = (d1, d2) =>
    d1.getDate() === d2.getDate()
    && d1.getMonth() === d2.getMonth()
    && d1.getFullYear() === d2.getFullYear();

const datesRange = (startDate, endDate) => {
    let dates = [];

    for (let d = new Date(startDate); d < endDate; d.setDate(d.getDate() + 1)) {
        dates.push(new Date(d));
    }

    return dates;
}

const formatDate = (date) => {
    let day = date.getDate();
    let month = date.getMonth() + 1;
    let year = date.getFullYear();

    return `${day < 10 ? '0' : ''}${day}`
        + `.${month < 10 ? '0' : ''}${month}`
        + `.${year}`;
}

const buildLessonString = (lesson, isGroup) => {
    let str = '';
    str += (isGroup && lesson.subgroup) ? `${lesson.subgroup}.&nbsp;` : '';
    str += `${lesson.subject}`;
    str += ` (${lesson.type})`;
    str += isGroup ? `<br/>${lesson.teacher}` : `<br/>Группа: ${lesson.group}`;
    str += (!isGroup && lesson.subgroup) ? `<br/>Подгруппа: ${lesson.subgroup}` : '';
    return str;
}

const buildRoomString = (lesson, isGroup) => {
    return `${isGroup && lesson.subgroup ? `${lesson.subgroup}.&nbsp;` : ''}${lesson.room}`;
}

function DateRow(props) {
    return (
        <tr>
            <th scope='col' rowSpan='2'>№</th>
            {props.dates.map((d, i) => <th key={i} scope='col' colSpan='2'>{formatDate(d)}</th>)}
        </tr>
    );
}

function DescriptionRow(props) {
    let heads = [];

    for (let i = 0; i < props.daysCount; i++) {
        heads.push('Дисциплина', 'Ауд.');
    }

    return (
        <tr>
            {heads.map((h, i) => <th key={i} scope='col'>{h}</th>)}
        </tr>
    );
}

function TableBodyRow(props) {
    let cells = [];

    props.dates.forEach(d => {
        let lessonsOnDate = props.lessons.filter(l => dateEqualityComparer(l.date, d));

        let lessonCell = lessonsOnDate.map(l => buildLessonString(l, props.isGroup)).join('<br/>');
        let roomCell = lessonsOnDate.map(l => buildRoomString(l, props.isGroup)).join('<br/>');

        if (!lessonCell) lessonCell = '-';
        if (!roomCell) roomCell = '-';

        cells.push(lessonCell, roomCell);
    });

    return (
        <tr>
            <th scope='row'>{props.number}</th>
            {cells.map((c, i) =>
                <td key={i}><p dangerouslySetInnerHTML={{ __html: c }} /></td>
            )}
        </tr>
    );
}

export function WeeklyTable(props) {
    let { isGroup, title, schedule } = props;
    let startDate = new Date(props.startDate.toDateString());
    let endDate = new Date(props.endDate.toDateString());
    let dates = datesRange(startDate, endDate);

    schedule.map(l => l.date = new Date(l.date));

    let h2 = isGroup ? `Группа - ${title}` : `Преподаватель - ${title}`;
    let h3 = `${formatDate(startDate)} - ${formatDate(endDate)}`;

    let minNumber = Math.min(...schedule.map(l => parseInt(l.number)));
    let maxNumber = Math.max(...schedule.map(l => parseInt(l.number)));

    return (
        <div>
            <h2>{h2}</h2>
            <h3>{h3}</h3>
            <table className='table'>
                <thead>
                    <DateRow dates={dates} />
                    <DescriptionRow daysCount={dates.length} />
                </thead>
                <tbody>
                    {range(minNumber, maxNumber).map(i =>
                        <TableBodyRow key={i}
                            lessons={schedule.filter(l => l.number === i)}
                            dates={dates}
                            number={i}
                            isGroup={isGroup} />
                    )}
                </tbody>
            </table>
        </div>
    );
}
