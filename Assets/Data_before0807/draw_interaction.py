import matplotlib.pyplot as plt
from statistics import mean
import numpy as np
from matplotlib.legend_handler import HandlerTuple

time_guide_list = []
time_noguide_list = []
success_guide_list = []
success_noguide_list = []
iterations = 3

file_pre = 'interaction_record_'
file_suf = '.txt'

for i in range(1, iterations+1):
    file_name = file_pre + str(i) + file_suf
    print(file_name)
    time_guide = []
    time_noguide = []
    success_guide = 0
    success_noguide = 0
    with open(file_name, 'r') as file:
        lines = file.readlines()[1:]

    iter = 0
    prev_parts = None
    true_flag = False
    for j in range(len(lines)):
        line = lines[j].rstrip()
        parts = line.split(', ')
        if parts[2] != iter:
            iter = parts[2]
            prev_parts = None
            true_flag = False
        if parts[4] == "True" and not true_flag:
            true_flag = True
            time_activate = float(parts[3])
            time_hover = 0
            if prev_parts != None and prev_parts[4] == "Hover":
                time_hover = float(prev_parts[3])
            if parts[1] == "True":
                time_guide.append(time_activate-time_hover)
                success_guide += 1
            else:
                time_noguide.append(time_activate-time_hover)
                success_noguide += 1
        prev_parts = parts
    time_guide_list.append(mean(time_guide))
    time_noguide_list.append(mean(time_noguide))
    success_guide_list.append(success_guide/5.0)
    success_noguide_list.append(success_noguide/5.0)

# guide1 = [0.968363, 2.399828, 0.834137, 0.73128, 1.302604]
# noguide1 = [1.192796-0.3117679, 1.14765-0.214009, 0.9899997-0.1706218, 1.143745-0.0535892, 1.119412-0]
# guide2 = [2.387852-0.3580014, 1.255637-0.1728584, 1.467991-0, 3.252087-0.2720174, 1.963238-0.0556464]
# noguide2 = [1.188677-0.2773853, 1.086023-0.2758193, 0.9134721-0, 1.012179-0.161444, 1.199811-0.2409386]
# guide3 = [1.166231-0.3283819, 0.9323627-0.2785782, 1.135594-0.3069492, 1.933567-0.250709, 0.7846164-0]
# noguide3 = [1.29147-0.4031971, 1.112905-0.332592, 1.003135-0.2462151, 1.370093-0.5160167, 1.183431-0.3050825]

# guideSuc = [1, 1, 1]
# noguideSuc = [1, 1, 1]

# guide_activate_time = [mean(guide1), mean(guide2), mean(guide3)]
# noguide_activate_time = [mean(noguide1), mean(noguide2), mean(noguide3)]

x = [i for i in range(1, iterations+1)]

fig, ax1 = plt.subplots()

line1, = ax1.plot(x, success_guide_list, color='r', linewidth=0.5, label='Guidance', linestyle='--')
line2, = ax1.plot(x, success_noguide_list, color='r', linewidth=0.5, label='No Guidance')
ax1.set_ylabel('Success Rate', color='r')
ax1.set_xlabel('Round')
ax1.set_ylim([0, 1.1])
ax1.set_xticks(x)

ax2 = ax1.twinx()

line3, = ax2.plot(x, time_guide_list, color='b', linewidth=0.5, label='Guidance', linestyle='--')
line4, = ax2.plot(x, time_noguide_list, color='b', linewidth=0.5, label='No Guidance')
ax2.set_ylabel('Activate Time', color='b')

ax1.scatter(x, success_guide_list, color='red')
ax1.scatter(x, success_noguide_list, color='red')
ax2.scatter(x, time_guide_list, color='blue')
ax2.scatter(x, time_noguide_list, color='blue')

handler = HandlerTuple(ndivide=None)

legend = plt.legend([(line1, line3), (line2, line4)], ['Guidance', 'No Guidance'], handler_map={tuple: handler})

plt.gca().add_artist(legend)

plt.savefig('interaction.png', dpi=300)
