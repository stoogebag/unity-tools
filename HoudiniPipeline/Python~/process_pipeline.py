import hou
import json
import os
import sys

def main():
    if len(sys.argv) < 4:
        print("Usage: hython process_pipeline.py <pipeline_folder> <hip_file> <parent_path>")
        sys.exit(1)

    pipeline_folder = sys.argv[1]
    hip_file = sys.argv[2]
    parent_path = sys.argv[3]

    control_path = os.path.join(pipeline_folder, "control.json")
    if not os.path.exists(control_path):
        print(f"control.json not found: {control_path}")
        sys.exit(1)

    with open(control_path) as f:
        ctrl = json.load(f)

    print(f"Loading: {hip_file}")
    hou.hipFile.load(hip_file, ignore_load_warnings=True)

    parent = hou.node(parent_path)
    if parent is None:
        print(f"Parent node not found: {parent_path}")
        sys.exit(1)

    control_path = parent.path() + "/unity_control"
    control = hou.node(control_path)
    if control is None:
        print(f"unity_control not found at: {control_path}")
        sys.exit(1)

    for parm_name, json_key in [("input_usd_path", "input_usd"), ("output_usd_path", "output_usd")]:
        parm = control.parm(parm_name)
        if parm is not None:
            val = ctrl.get(json_key, "")
            parm.set(val)
            print(f"  {parm_name} = {val}")

    parm = control.parm("control_json")
    if parm is not None:
        parm.set(json.dumps(ctrl))

    # Find the USD export node and cook it to write the output
    export_node = parent.node("usdexport1")
    if export_node is None:
        # Try recursive search
        for child in parent.allSubChildren():
            if "usdexport" in child.name().lower():
                export_node = child
                break

    if export_node is None:
        print("No USD export node found (usdexport1 or similar)")
        sys.exit(1)

    print(f"Exporting: {export_node.path()}")
    export_node.parm("execute").pressButton()

    print("Done.")
    sys.exit(0)


if __name__ == "__main__":
    main()
