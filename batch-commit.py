import subprocess

# get files from git
all_files = subprocess.check_output(
    "git status --porcelain -u", shell=True, universal_newlines=True)

# dump = open("commit-dump.txt", "w")
# _ = dump.write(all_files)
# dump.close()


# split total files into list
individual_files = all_files.split("\n")

# try to commit and push while we got files left
while(len(individual_files) > 0):
    print(f"total count is: {len(individual_files)}")
    # our num of files to fetch per push, limited to total files left
    end = min(100, len(individual_files))
    temp = individual_files[:end]
    print(f"committing: {end}")

    # truncate list
    individual_files = individual_files[end:]

    # add all files in subset
    for line in temp:
        subprocess.run(f"git add {line[3:]}")

    subprocess.run("git commit -m \"automated asset push\"")
    subprocess.run("git push")
